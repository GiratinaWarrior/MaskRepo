using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private int PlayerSpeed = 10;
    [SerializeField] private Rigidbody2D rb;

    private Vector3 moveVec = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    { 
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
        moveVec = (Vector3) context.ReadValue<Vector2>() * PlayerSpeed;
    }
}
