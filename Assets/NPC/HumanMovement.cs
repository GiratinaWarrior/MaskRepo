using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class HumanMovement : MonoBehaviour
{
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
        //transform.position += new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0) * Random.Range(1, 5) * Time.deltaTime;
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
        if (collision.gameObject.name == "Background")
        {
            Destroy(gameObject);
        }
    }
}
