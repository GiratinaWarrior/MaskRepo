using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private float DampRate = 25;
    [SerializeField] private float snapDist = 0.1f;

    private const int cameraZ = -10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player != null)
        {
            //Camera follows Player
            transform.position += ((player.transform.position - transform.position) / DampRate);

            if (Vector3.Distance(transform.position, player.transform.position) < snapDist)
            {
                transform.position = player.transform.position;
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, cameraZ);
        }

    }
}
