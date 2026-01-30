using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class HumanMovement : MonoBehaviour
{
    private Vector2 _targetPosition;
    private float timer = 0f;
    private float idleTime = 3f;
    private char _state = 'i';
    [SerializeField] GameObject _detection;


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
                    _state = 'w';
                    timer = 0f;
                    _targetPosition = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                }
                break;
            case 'w':
                Vector2 movement = Vector2.MoveTowards(transform.position, _targetPosition, Time.deltaTime);
                _detection.transform.Rotate(transform.position, Mathf.Atan2(movement.x, movement.y));
                transform.position = movement;
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
           
            //add score
            player.updateScareCount();



            //HUMAN RUNS AWAY OR SOMETHING
            //****TO DO****
            

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.name == "Background")
        //{
        //    Destroy(gameObject);
        //}
    }
}
