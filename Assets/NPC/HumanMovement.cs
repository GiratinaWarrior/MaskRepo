using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UI.Image;

public class HumanMovement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer human_sprite;
    [SerializeField] private Sprite spr_idle;
    [SerializeField] private Sprite spr_wander;
    [SerializeField] private Sprite spr_scared;
    [SerializeField] private Sprite spr_dead;

    [SerializeField] private Animator animator;

    [SerializeField] Vector3 _targetPosition;
    private float timer = 0f;
    private float idleTime = 3f;
    private bool justTransitioned = true;

    enum HUMAN_STATE
    {
        Idle,
        Wander,
        Scared,
        Suspicious,
        Dead
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponentInChildren<DetectionComponent>().PlayerDetected += OnPlayerSpotted;
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
        switch (human_state)
        {
            //Idle
            case HUMAN_STATE.Idle:
                timer += Time.fixedDeltaTime;

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
            //Wander
            case HUMAN_STATE.Wander:
                moveVec = _targetPosition - transform.position;
                moveVec.Normalize();
                _detection.transform.localEulerAngles = new Vector3(0, 0, VectorToAngle(moveVec));
                //_detection.transform.rotation = Quaternion.LookRotation((Vector3)_targetPosition - transform.position);
                //_detection.transform.rotation = Quaternion.LookRotation(Vector3.forward, movement - (Vector2)transform.position);
                Debug.DrawRay(transform.position, _targetPosition - transform.position,Color.green);
                if (Vector2.Distance(transform.position, _targetPosition) < 1f)
                {
                    human_state = HUMAN_STATE.Idle;
                    timer = 0f;
                    moveVec = Vector3.zero;
                }
                break;
            case HUMAN_STATE.Suspicious:
                _detection.transform.localEulerAngles = new Vector3(0, 0, VectorToAngle(spottedPlayer.position - transform.position));
                if ((spottedPlayer.position - transform.position).x < 0)
                {
                    human_sprite.flipX = true;
                }
                else 
                    human_sprite.flipX = false;
                break;
            //Scared
            case HUMAN_STATE.Scared:
                _detection.transform.localEulerAngles = new Vector3(0, 0, VectorToAngle(moveVec));
                break;

            case HUMAN_STATE.Dead:

                

                break;
        }
        transform.position += moveVec * Time.deltaTime;

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

        public void PlayAnimation(string animName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
            animator.Play(animName, 0, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If this human collides with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

            //Debug.Log("Human Scary touched");
           
            //HUMAN RUNS AWAY OR SOMETHING
            //****TO DO****
            spottedPlayer = player.transform;
            if (player.PlayerIsScary() && !isSpooked)
            {
                player.updateScareCount();


            moveVec = Vector3.Normalize(transform.position - player.transform.position)*scaredRunSpeed;
            gameObject.GetComponent<FadeDestroy>().enabled = true;

                isSpooked = true;
                human_state = HUMAN_STATE.Scared;
            }
            else if (player.PlayerIsKilling())
            {
                KillHuman();
            }
        }
    }

    private void OnPlayerSpotted(Transform player)
    {
        spottedPlayer = player;
        Debug.Log("Player spotted by human " + player.gameObject.name);
        if (human_state != HUMAN_STATE.Scared)
            human_state = HUMAN_STATE.Suspicious;
    }

    private void KillHuman()
    {
        human_state = HUMAN_STATE.Dead;
        _detection.SetActive(false);
        SetSprite(spr_dead);
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
