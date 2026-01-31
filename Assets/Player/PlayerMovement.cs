using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    //Components
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer player_sprite;
    [SerializeField] private CircleCollider2D abilityTrigger;

    [SerializeField] private GameObject shoutShockwave;

    [SerializeField] private int PlayerSpeed = 10;
    private Vector3 moveVec = Vector3.zero;

    [SerializeField] private Sprite spr_normal;
    [SerializeField] private Sprite spr_scare;
    [SerializeField] private Sprite spr_kill;

    [SerializeField] private int ScareDuration = 60;
    private int ScareTimer = 0;

    [SerializeField] private int ScareCooldown = 60;
    private int ScareCooldownTimer = 0;

    [SerializeField] private int KillDuration = 30;
    private int KillTimer = 0;

    [SerializeField] private int KillCooldown = 40;
    private int KillCooldownTimer = 0;

    public int score = 0;
    private int scareCount = 0;

    private bool InKillRange = false;

    enum PLAYER_STATE
    {
        Normal,
        Scare,
        Kill
    }

    private PLAYER_STATE player_state = PLAYER_STATE.Normal;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player_sprite = GetComponent<SpriteRenderer>();
        abilityTrigger = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (player_state)
        {
            //Monster is just wandering around
            case PLAYER_STATE.Normal:

                ScareCooldownTimer = Mathf.Max(0, ScareCooldownTimer - 1);

                KillCooldownTimer = Mathf.Max(0, KillCooldownTimer - 1);
                
                break;

            //Monster uses JUMPSCARE, frightening other players
            case PLAYER_STATE.Scare:

                ScareTimer = Mathf.Max(0, ScareTimer - 1);

                if (ScareTimer == 0)
                {
                    ScareDeactivate();
                }

                break;
            
            //Monster kills player
            case PLAYER_STATE.Kill:

                KillTimer = Mathf.Max(0, KillTimer - 1);

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

    private void SetSprite(Sprite spr)
    {
        player_sprite.sprite = spr;
    }

    //GetMove(context) gets the players input for movement,
    //and if the user has given input, and the game is still ongoing
    //the player will change directions based on the given input
    public void GetMove(InputAction.CallbackContext context)
    {
        moveVec = (Vector3)context.ReadValue<Vector2>() * PlayerSpeed;
    }//GetMove

    public void JumpScare()
    {
        if (player_state == PLAYER_STATE.Normal && ScareCooldownTimer <= 0)
        {
            ScareActivate();
        }
    }//JumpScare

    public void Kill()
    {
        if (InKillRange || true)
        {
            KillActivate();
        }
    }

    public void updateScareCount()
    {
        scareCount++;
        //scoreText.text = "Scare Count: " + scareCount;
    }

    public bool PlayerIsScary()
    {
        return (player_state == PLAYER_STATE.Scare);
    }

    public bool PlayerIsKilling()
    {
        return (player_state == PLAYER_STATE.Kill);
    }

    private void ScareActivate()
    {
        player_state = PLAYER_STATE.Scare;
        ScareTimer = ScareDuration;
        SetSprite(spr_scare);
        abilityTrigger.enabled = true;
        Instantiate(shoutShockwave, transform.position, Quaternion.identity);
    }

    private void ScareDeactivate()
    {
        player_state = PLAYER_STATE.Normal;
        SetSprite(spr_normal);
        ScareCooldownTimer = ScareCooldown;
        abilityTrigger.enabled = false;
    }

    private void KillActivate()
    {
        player_state = PLAYER_STATE.Kill;
        SetSprite(spr_kill);
        KillTimer = KillDuration;
        abilityTrigger.enabled = true;
    }

    private void KillDeactivate()
    {
        player_state = PLAYER_STATE.Normal;
        SetSprite(spr_normal);
        KillCooldownTimer = KillCooldown;
        abilityTrigger.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //When the player is close enough to attack
        if (collision.gameObject.CompareTag("Human"))
        {
            InKillRange = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //When the player is close enough to attack
        if (collision.gameObject.CompareTag("Human"))
        {
            InKillRange = false;
        }
    }

    


}