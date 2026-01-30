using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject human;

    [SerializeField] private int HumanSpawnRate = 80;
    private int HumanSpawnTimer = 0;

    [SerializeField] private float SpawnRange = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        Instantiate(player, new Vector3(0, 0, 0), transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {

        HumanSpawnTimer = Mathf.Max(0, HumanSpawnTimer - 1);

        if (HumanSpawnTimer == 0)
        {
            Vector3 offsetVec = new Vector3(Random.Range(-SpawnRange, SpawnRange), Random.Range(-SpawnRange, SpawnRange), 0);
            Instantiate(human, player.transform.position + offsetVec, transform.rotation);
            HumanSpawnTimer = HumanSpawnRate;
        }
    }
}
