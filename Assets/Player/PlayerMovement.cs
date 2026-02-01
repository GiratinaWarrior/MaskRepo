using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
   
    

    //Components
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer player_sprite;
    [SerializeField] private CircleCollider2D abilityTrigger;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject shoutShockwave;
    public GameManager gameManager;
    

    [SerializeField] private int PlayerSpeed = 10;
    private float currSpeed = 10;
    private Vector3 moveVec = Vector3.zero;

    //[SerializeField] private Sprite spr_normal;
    //[SerializeField] private Sprite spr_scare;
    //[SerializeField] private Sprite spr_kill;

    enum FACING_DIR
    {
        Side,
        Up,
        Down
    }

    private FACING_DIR facingDir = FACING_DIR.Down;

    //---------------Parameters for the Abilities

    [SerializeField] private float ScareDuration = 60;
    private float ScareTimer = 0;

    [SerializeField] private float ScareCooldown = 60;
    private float ScareCooldownTimer = 0;

    [SerializeField] private float KillDuration = 30;
    private float KillTimer = 0;

    [SerializeField] private float KillCooldown = 80;
    private float KillCooldownTimer = 0;

    //private float killChargeTime = 1.5f;
    private float chargeTimer = 0f;

    private bool InKillRange = false;

    //------------Points for the various scares

    public int scareCount = 0;
    public int superScare = 0;
    public int killCount = 0;
    public int approachCount = 0;

    enum PLAYER_STATE
    {
        Normal,
        Scare,
        Charge_Up,
        Kill
    }

    private PLAYER_STATE player_state = PLAYER_STATE.Normal;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player_sprite = GetComponent<SpriteRenderer>();
        abilityTrigger = GetComponent<CircleCollider2D>();
        abilityTrigger.radius = 2;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateAnimation();
        switch (player_state)
        {
            //Monster is just wandering around
            case PLAYER_STATE.Normal:

                //-------Update the cooldown timers for the abilities

                ScareCooldownTimer = Mathf.Max(0, ScareCooldownTimer - Time.deltaTime);

                KillCooldownTimer = Mathf.Max(0, KillCooldownTimer - Time.deltaTime);
                
                break;

            //Monster uses JUMPSCARE, frightening other players
            case PLAYER_STATE.Scare:

                ScareTimer = Mathf.Max(0, ScareTimer - Time.deltaTime);

                if (ScareTimer == 0)
                {
                    ScareDeactivate();
                }

                break;

            //Monster kills player
            case PLAYER_STATE.Charge_Up:
                currSpeed = 5;
                chargeTimer += Time.deltaTime;

                break;
            case PLAYER_STATE.Kill:

                KillTimer = Mathf.Max(0, KillTimer - Time.deltaTime);

                if (KillTimer == 0)
                {
                    KillDeactivate();
                }
                break;

        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveVec;
    }

    //private void SetSprite(Sprite spr)
    //{
    //    player_sprite.sprite = spr;
    //}

    //GetMove(context) gets the players input for movement,
    //and if the user has given input, and the game is still ongoing
    //the player will change directions based on the given input
    public void GetMove(InputAction.CallbackContext context)
    {
        if (gameManager.GameActive()) moveVec = (Vector3)context.ReadValue<Vector2>() * PlayerSpeed;
        else moveVec = Vector3.zero;
    }//GetMove

    private void UpdateMovementAnimation()
    {
        // Only animate movement in Normal state
        if (player_state != PLAYER_STATE.Normal)
            return;

        // Idle
        if (moveVec == Vector3.zero)
        {

            PlayAnimation("Idle");
            return;
        }


        // Vertical movement has priority
        if (Mathf.Abs(moveVec.y) > Mathf.Abs(moveVec.x))
        {
            if (moveVec.y > 0)
            {
                facingDir = FACING_DIR.Up;
                PlayAnimation("Walk_Up");
            }
            else
            {
                facingDir = FACING_DIR.Down;
                PlayAnimation("Walk_Down");
            }
        }
        else
        {
            facingDir = FACING_DIR.Side;
            PlayAnimation("Walk");
        }
        // Flip sprite
        if (moveVec.x < 0)
            player_sprite.flipX = true;
        else if (moveVec.x > 0)
            player_sprite.flipX = false;
    }

    private void UpdateAnimation()
    {
        switch (player_state)
        {
            case PLAYER_STATE.Normal:
                UpdateMovementAnimation();
                break;

            case PLAYER_STATE.Scare:
                switch (facingDir)
                {
                    case FACING_DIR.Up:
                        PlayAnimation("Scare_Up");
                        break;

                    case FACING_DIR.Down:
                        PlayAnimation("Scare_Down");
                        break;

                    case FACING_DIR.Side:
                        PlayAnimation("Scare");
                        break;
                }
                break;

            case PLAYER_STATE.Kill:
                //PlayAnimation("Kill");
                break;
        }
    }

    public void PlayAnimation(string animName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
                animator.Play(animName, 0, 0f);
    }

    //JumpScare() is called upon pressing the jumpscare button, and checks the necessary conditions for scaring
    public void JumpScare()
    {
        if (gameManager.GameActive() && player_state == PLAYER_STATE.Normal && ScareCooldownTimer <= 0)
        {
            ScareActivate();
        }
    }//JumpScare
    
    //-------------The following update the various score parameters
    
    public void UpdateScareCount(bool surprise = false)
    {
        scareCount += (surprise ? 2 : 1);// Mathf.Max(1, Mathf.FloorToInt(susLevel * 3));
    }

    public void UpdateKillCount()
    {
        killCount += 1;// Mathf.Max(1, Mathf.FloorToInt(susLevel * 3));
    }

    public void UpdateApproachCount()
    {
        approachCount++;
    }

    public void ResetCount()
    {
        scareCount = 0;
        killCount = 0;
    }

    //-----------The following functions check the current state of the Player

    public bool PlayerIsScary()
    {
        return (player_state == PLAYER_STATE.Scare);
    }

    public bool PlayerIsKilling()
    {
        return (player_state == PLAYER_STATE.Kill);
    }

    //---------------The following activate the Players abilities

    //ScareActivate() starts the players jumpscare
    private void ScareActivate()
    {
        player_state = PLAYER_STATE.Scare;
        ScareTimer = ScareDuration; 
        abilityTrigger.enabled = true;
        abilityTrigger.radius = 2;
        Instantiate(shoutShockwave, transform.position, Quaternion.identity);
    }

    //ScareDeactivate() ends the players jumpscare
    private void ScareDeactivate()
    {
        player_state = PLAYER_STATE.Normal;
        //SetSprite(spr_normal);
        ScareCooldownTimer = ScareCooldown;
        abilityTrigger.enabled = false;
    }

    //KillActivate() starts the players killing strike
    public void KillActivate()
    {
        if (InKillRange)
        {
            player_state = PLAYER_STATE.Kill;
            KillTimer = KillDuration;
            abilityTrigger.enabled = true;
            abilityTrigger.radius = 1;
            GameObject killWave = Instantiate(shoutShockwave, transform.position, Quaternion.identity);
            killWave.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    //KillActivate() ends the players killing strike
    private void KillDeactivate()
    {
        player_state = PLAYER_STATE.Normal;
        //SetSprite(spr_normal);
        KillCooldownTimer = KillCooldown;
        abilityTrigger.enabled = false;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //When the player is close enough to attack
        if (collision.gameObject.CompareTag("Human"))
        {
            InKillRange = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Human"))
        {
            InKillRange = false;
        }
    }



}