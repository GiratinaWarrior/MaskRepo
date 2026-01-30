using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(player, new Vector3(0, 0, 0), transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
