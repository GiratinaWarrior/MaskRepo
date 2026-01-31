using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class HumanMovement : MonoBehaviour
{
    private Vector2 _targetPosition;
    private float timer = 0f;
    private float idleTime = 3f;
    private char _state = 'i';
    [SerializeField] GameObject _detection;



    private bool isSpooked = false;
    public int scaredRunSpeed = 5;
    private Vector3 runDirVec = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        switch (_state)
        {
            case 'i':
                timer += Time.fixedDeltaTime;
                if (timer >= idleTime)
                {
                    timer = 0f;
                    _state = 'w';
                    //Pick a random target position within some radius
                    float randX = Random.Range(-5f, 5f);
                    float randY = Random.Range(-5f, 5f);
                    _targetPosition = new Vector2(transform.position.x + randX, transform.position.y + randY);
                }
                break;
            case 'w':
                Vector2 movement = Vector2.MoveTowards(transform.position, _targetPosition, Time.deltaTime);
                _detection.transform.rotation = Quaternion.LookRotation(Vector3.forward, movement - (Vector2)transform.position);
                transform.position = movement;
                break;
            case 's':
                transform.position += runDirVec * scaredRunSpeed * Time.deltaTime;
                break;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If this human collides with the player
        if (collision.gameObject.tag == "Player")
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

            Debug.Log("Human Scary touched");
           
            //HUMAN RUNS AWAY OR SOMETHING
            //****TO DO****
            if (!isSpooked)
            {
                player.updateScareCount();

                
                runDirVec = Vector3.Normalize(transform.position - player.transform.position);
                gameObject.GetComponent<FadeDestroy>().enabled = true;

                isSpooked = true;
                _state = 's';
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.name == "Background")
        //{
        //    Destroy(gameObject);
        //}
    }

    private void move(Vector3 movement)
    {
        transform.position += movement * Time.deltaTime;

    }
}
