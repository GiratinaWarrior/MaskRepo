using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UI.Image;

public class HumanMovement : MonoBehaviour
{
    //All the sprites of the human
    [SerializeField] private SpriteRenderer human_sprite;
    [SerializeField] private Sprite spr_idle;
    [SerializeField] private Sprite spr_wander;
    [SerializeField] private Sprite spr_scared;
    [SerializeField] private Sprite spr_dead;

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] Vector3 _targetPosition;
    private float timer = 0f;
    private float idleTime = 3f;
    //private bool justTransitioned = true;

    

    //State machine for the humans different behaviours
    enum HUMAN_STATE
    {
        Idle, //Human stands in one spot
        Wander, //Human wanders around
        Scared, //Human runs in fear of their lives
        Suspicious, //Humans freezes in fear as they notice something
        Dead //Human becomes a corpse that does nothing
    }

    private HUMAN_STATE human_state = HUMAN_STATE.Idle;
    [SerializeField] GameObject _detection;
    [SerializeField] GameObject dead_shriek;
    Transform spottedPlayer;

    Vector3 moveVec = Vector3.zero;

    private float flashlightSize = 2;
    
    private bool isSpooked = false;
    public int scaredRunSpeed = 2;
    private Vector3 runDirVec = Vector3.zero;

    public GameManager gameManager;

    //The distance a human in the 'suspicious' state will notice the player is a monster and run
    [SerializeField] private float AlertDistance = 1;

    public float maxSus = 20;
    public float susMeter = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponentInChildren<DetectionComponent>().PlayerDetected += OnPlayerSpotted;
        GetComponentInChildren<DetectionComponent>().PlayerLost += OnPlayerLost;

        human_sprite = GetComponent<SpriteRenderer>();
        flashlightSize = _detection.transform.localScale.x;
        _detection.transform.localEulerAngles = new Vector3(0, 0, VectorToAngle(Vector3.left));
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        //-----------------------------HUMAN STATE MACHINE--------------------//
        switch (human_state)
        {
            //--------------------------IDLE
            case HUMAN_STATE.Idle:
                timer += Time.fixedDeltaTime;

                susMeter = Mathf.Max(0, susMeter - 1);

                //When the human is done idling, choose a point to travel to and go there
                if (timer >= idleTime)
                {
                    timer = 0f;
                    human_state = HUMAN_STATE.Wander;
                    //Pick a random target position within some radius
                    float randX = Random.Range(-5f, 5f);
                    float randY = Random.Range(-5f, 5f);
                    _targetPosition = new Vector3(transform.position.x + randX, transform.position.y + randY, 0);
                }
                break;

            //------------------------WANDER
            case HUMAN_STATE.Wander:

                //Human moves towards the target destination
                moveVec = _targetPosition - transform.position;
                moveVec.Normalize();

                //Humans flashlight shines in the direction they travel in
                _detection.transform.localEulerAngles = new Vector3(0, 0, VectorToAngle(moveVec));

                //Human goes back to idling once they are close enough to their destination
                if (Vector2.Distance(transform.position, _targetPosition) < 1f)
                {
                    human_state = HUMAN_STATE.Idle;
                    timer = 0f;
                    moveVec = Vector3.zero;
                }

                break;

            //--------------------SUSPICIOUS
            case HUMAN_STATE.Suspicious:

                susMeter = Mathf.Min(susMeter + 1, maxSus);

                //Flashlight is kept in the players direction
                _detection.transform.localEulerAngles = new Vector3(0, 0, VectorToAngle(spottedPlayer.position - transform.position));

                //The distance the player currently is from the human
                float DistFromPlayer = Vector3.Distance(transform.position, spottedPlayer.position);

                //If the player is close enough for the human to notice, they run away scared
                if (DistFromPlayer <= AlertDistance)
                {
                    HumanBecomeScared(spottedPlayer.gameObject.GetComponent<PlayerMovement>(), false);
                }

                //Flip the players sprite
                if ((spottedPlayer.position - transform.position).x < 0) human_sprite.flipX = true; 
                else human_sprite.flipX = false;

                break;
            //---------------------SCARED
            case HUMAN_STATE.Scared:
                //flash is placed in the direction of running
                _detection.transform.localEulerAngles = new Vector3(0, 0, VectorToAngle(moveVec));
                break;

            //---------------------DEAD
            case HUMAN_STATE.Dead:
                //Player cannot move while dead
                moveVec = Vector3.zero;

                break;
        }
        //transform.position += moveVec * Time.deltaTime;
        rb.linearVelocity = moveVec;
    }

    public void UpdateAnimation()
    {

        switch (human_state)
        {
            case HUMAN_STATE.Idle:
                PlayAnimation("Idle");
                break;
            case HUMAN_STATE.Wander:
                PlayAnimation("Walk");
                break;
            case HUMAN_STATE.Scared:
                float humanAngle = Mathf.Atan2(transform.position.y - spottedPlayer.position.y, transform.position.x - spottedPlayer.position.x);
                if (humanAngle <= Mathf.PI / 4f && humanAngle >= Mathf.PI / -4f || (humanAngle >= Mathf.PI * 3f / 4f || humanAngle <= Mathf.PI * -3f / 4f))
                    PlayAnimation("Scared");
                else if (humanAngle >= Mathf.PI / 4f && humanAngle <= Mathf.PI * 3f / 4f)
                    PlayAnimation("Scared_Up");
                else if (humanAngle >= Mathf.PI * -3f / 4f && humanAngle <= Mathf.PI / -4f)
                    PlayAnimation("Scared_Down");
                break;
            case HUMAN_STATE.Suspicious:
                PlayAnimation("Idle");
                break;
            case HUMAN_STATE.Dead:
                PlayAnimation("Dead");
                break;
        }
        if (moveVec.x < 0)
        {
            human_sprite.flipX = true;
        }
        else if (moveVec.x > 0)
        {
            human_sprite.flipX = false;
        }
    }

    public bool HumanAlive()
    {
        return human_state != HUMAN_STATE.Dead;
    }

    public bool HumanScared()
    {
        return human_state == HUMAN_STATE.Scared;
    }

    public bool HumanSuspicious()
    {
        return human_state == HUMAN_STATE.Suspicious;
    }

    public void PlayAnimation(string animName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
            animator.Play(animName, 0, 0f);
    }

    //OnTriggerEnter2D(collision) will be called when the player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If this human collides with the player
        if (human_state != HUMAN_STATE.Dead && collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

             
            spottedPlayer = player.transform;

            //If the player is jumpscaring the human and the human hasn't been jumpscared yet
            if (player.PlayerIsScary() && (human_state != HUMAN_STATE.Scared))
            { 
                HumanBecomeScared(player, true, human_state == HUMAN_STATE.Idle);
            }

            //If the player is killing the human
            else if (player.PlayerIsKilling())
            {
                KillHuman();
                player.UpdateKillCount();
            }
        }
        else if (collision.gameObject.CompareTag("KillBox"))
        {
            Debug.Log(collision.gameObject.tag);
            KillHuman();
            gameManager.myPlayer.UpdateKillCount();
            Debug.Log("killed by: " + collision.gameObject.name);

        }
    }

    //HumanBecomeScared(player, isJumpScare) tells the player to become scared
    //isJumpScared: Boolean that is true if the player is running because the player jumpscared them, else false
    public void HumanBecomeScared(PlayerMovement player, bool isJumpScare = true, bool surprise = false)
    {
        //update the corresponding score counter
        if (isJumpScare)
        {
            player.UpdateScareCount(surprise);
        }
        else player.UpdateApproachCount();

        //Human moves in direction opposite of player
        moveVec = Vector3.Normalize(transform.position - player.transform.position) * scaredRunSpeed;
        gameObject.GetComponent<FadeDestroy>().enabled = true;

        isSpooked = true;
        human_state = HUMAN_STATE.Scared;
    }

    private void OnPlayerSpotted(Transform player)
    {
        spottedPlayer = player;
        //Debug.Log("Player spotted by human " + player.gameObject.name);
        if (human_state != HUMAN_STATE.Scared) human_state = HUMAN_STATE.Suspicious;
    }
    private void OnPlayerLost()
    {
        human_state = HUMAN_STATE.Idle;
        spottedPlayer = null;

    }
    //KillHuman() sets the human into the dead state, they become inert and the flashlight goes out
    private void KillHuman()
    {
        moveVec = Vector3.zero;
        human_state = HUMAN_STATE.Dead;
        _detection.SetActive(false);
        //SetSprite(spr_dead);
    }


    private void SetSprite(Sprite spr)
    {
        human_sprite.sprite = spr;
    }

    //VectorToAngle(vec) takes a vector vec and returns the angle formed by the x and y coordinates relative to the origin
    //Vector3 -> Float
    private float VectorToAngle(Vector3 vec)
    {
        return Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
    }

    //Vector3Rotate(vec, degrees) takes a 3d vector vec and rotates it by rotateAng degrees
    //Vector3 Float -> Vector3
    private Vector3 Vector3Rotate(Vector3 vec, float rotateAng)
    {
        float Angle = rotateAng * Mathf.Deg2Rad;
        return new(vec.x * Mathf.Cos(Angle) + vec.y * Mathf.Sin(Angle), vec.x * -Mathf.Sin(Angle) + vec.y * Mathf.Cos(Angle));
    }
}
