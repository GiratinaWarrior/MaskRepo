using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class HumanMovement : MonoBehaviour
{

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
        if (isSpooked) transform.position += runDirVec * scaredRunSpeed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        
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
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Background")
        {
            Destroy(gameObject);
        }
    }
}
