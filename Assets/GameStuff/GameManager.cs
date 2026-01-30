using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject human;
    public GameObject scoreTextObject;
    private TextMeshPro scoreTMP;

    [SerializeField] private int HumanSpawnRate = 80;
    private int HumanSpawnTimer = 0;

    [SerializeField] private float SpawnRange = 20;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreTMP = scoreTextObject.GetComponent<TextMeshPro>();
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
            SpawnHuman();
            HumanSpawnTimer = HumanSpawnRate;
        }
    }

    private void SpawnHuman()
    {
        Vector3 offsetVec = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0) * SpawnRange;
        Instantiate(human, player.transform.position + offsetVec, transform.rotation);
    }
}
