using UnityEngine;

public class Death_Shriek : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var human = collision.gameObject.GetComponent<HumanMovement>();
        if (human == null)
        {
            return;
        }
    }
}
