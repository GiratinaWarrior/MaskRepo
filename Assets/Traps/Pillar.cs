using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] int debrisAmount;
    [SerializeField] GameObject debrisPrefab;
    [SerializeField] float maxRadius;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player.PlayerIsScary())
            {
                for (int i = 0; i <= debrisAmount; i++)
                {
                    Vector3 random = Random.insideUnitSphere * maxRadius;
                    Instantiate(debrisPrefab, transform.position + random, Quaternion.identity);
                }
            }
        }
    }


    

}
