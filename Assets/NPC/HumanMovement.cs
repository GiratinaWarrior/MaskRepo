using UnityEngine;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.UI.Image;

public class HumanMovement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer human_sprite;
    [SerializeField] private Sprite spr_idle;
    [SerializeField] private Sprite spr_wander;
    [SerializeField] private Sprite spr_scared;
    [SerializeField] private Sprite spr_dead;

    private Vector2 _targetPosition;
    private float timer = 0f;
    private float idleTime = 3f;

    enum HUMAN_STATE
    {
        Idle,
        Wander,
        Scared,
        Dead
    }

    private HUMAN_STATE human_state = HUMAN_STATE.Idle;
    [SerializeField] GameObject _detection;

    private float flashlightSize = 2;


    private bool isSpooked = false;
    public int scaredRunSpeed = 5;
    private Vector3 runDirVec = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponentInChildren<DetectionComponent>().PlayerDetected += OnPlayerSpotted;
        human_sprite = GetComponent<SpriteRenderer>();
        flashlightSize = _detection.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
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
                    _targetPosition = new Vector2(transform.position.x + randX, transform.position.y + randY);
                }
                break;
            //Wander
            case HUMAN_STATE.Wander:
                Vector2 movement = Vector2.MoveTowards(transform.position, _targetPosition, Time.deltaTime);
                _detection.transform.eulerAngles = new Vector3(0, 0, VectorToAngle((Vector3)_targetPosition - transform.position));
                //_detection.transform.rotation = Quaternion.LookRotation((Vector3)_targetPosition - transform.position);
                //_detection.transform.rotation = Quaternion.LookRotation(Vector3.forward, movement - (Vector2)transform.position);
                transform.position = movement;
                break;
            //Scared
            case HUMAN_STATE.Scared:
                transform.position += scaredRunSpeed * Time.deltaTime * runDirVec;

                _detection.transform.eulerAngles = new Vector3(0, 0, VectorToAngle(runDirVec));
                break;

            case HUMAN_STATE.Dead:

                

                break;
        }

        if (runDirVec.x != 0)
        {
            int sign = runDirVec.x > 0 ? 1 : -1;
            gameObject.GetComponent<SpriteRenderer>().flipX = (sign == 1);
            //flashlight.transform.localScale = new Vector3(sign, 1, 1) * flashlightSize;
        }
        
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
            if (player.PlayerIsScary() && !isSpooked)
            {
                player.updateScareCount();


            runDirVec = Vector3.Normalize(transform.position - player.transform.position);
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


    private void move(Vector3 movement)
    {
        transform.position += movement * Time.deltaTime;

    }
    private void OnPlayerSpotted()
    {

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
