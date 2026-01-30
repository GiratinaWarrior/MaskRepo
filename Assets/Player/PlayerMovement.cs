using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    //Components
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer player_sprite;
    [SerializeField] private CircleCollider2D scareTrigger;

    [SerializeField] private int PlayerSpeed = 10;
    private Vector3 moveVec = Vector3.zero;

    [SerializeField] private Sprite spr_normal;
    [SerializeField] private Sprite spr_scare;

    [SerializeField] private int ScareDuration = 60;
    private int ScareTimer = 0;

    [SerializeField] private int ScareCooldown = 60;
    private int ScareCooldownTimer = 0;

    public int score = 0;
    private int scareCount = 0;

    enum PLAYER_STATE
    {
        Normal,
        Scare
    }

    private PLAYER_STATE player_state = PLAYER_STATE.Normal;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player_sprite = GetComponent<SpriteRenderer>();
        scareTrigger = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (player_state)
        {
            case PLAYER_STATE.Normal:

                ScareCooldownTimer = Mathf.Max(0, ScareCooldownTimer - 1);



                break;

            case PLAYER_STATE.Scare:

                ScareTimer = Mathf.Max(0, ScareTimer - 1);

                if (ScareTimer == 0)
                {
                    ScareDeactivate();
                }

                break;


        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveVec;
    }

    //GetMove(context) gets the players input for movement,
    //and if the user has given input, and the game is still ongoing
    //the player will change directions based on the given input
    public void GetMove(InputAction.CallbackContext context)
    {
        moveVec = (Vector3)context.ReadValue<Vector2>() * PlayerSpeed;
    }//GetMove

    public void GetAttack()
    {
        if (player_state == PLAYER_STATE.Normal && ScareCooldownTimer <= 0)
        {
            ScareActivate();
        }
    }//GetAttack

    public void updateScareCount()
    {
        scareCount++;
        //scoreText.text = "Scare Count: " + scareCount;
    }

    public bool PlayerIsScary()
    {
        return (player_state == PLAYER_STATE.Scare);
    }

    private void ScareActivate()
    {
        player_state = PLAYER_STATE.Scare;
        ScareTimer = ScareDuration;
        player_sprite.sprite = spr_scare;
        scareTrigger.enabled = true;
    }

    private void ScareDeactivate()
    {
        player_state = PLAYER_STATE.Normal;
        player_sprite.sprite = spr_normal;
        ScareCooldownTimer = ScareCooldown;
        scareTrigger.enabled = false;
    }


}