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
        transform.position += new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0) * Random.Range(1, 5) * Time.deltaTime;
    }
}
