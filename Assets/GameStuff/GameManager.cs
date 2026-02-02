using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Animations;
//using UnityEditor.Animations;

public class GameManager : MonoBehaviour
{
    //Player
    public GameObject player;
    public PlayerMovement myPlayer;

    //Human
    public GameObject human; 

    [SerializeField] private RuntimeAnimatorController[] variations;
    [SerializeField] private float HumanSpawnRate = 5;
    private float HumanSpawnTimer = 0;

    //How far away from the initial spawn point the humans can spawn
    [SerializeField] private float SpawnRange = 20;

    //Variables that store the UI components, the ones after the round ends
    //[SerializeField] private Text killText;
    //[SerializeField] private Text scareText;
    [SerializeField] private Text allScareScore;
    [SerializeField] private Text ratingText;
    [SerializeField] private GameObject ReviewScreen;
    [SerializeField] Light2D globalLight;

    [SerializeField] private Text scareScoreText;
    [SerializeField] private Text reviewScoreText;

    

    public enum GAME 
    {
        Off, //Player is given time to chill out
        Active, //Player can scare the humans now
        Over //The player lost (somehow)
    }

    private GAME game_state = GAME.Off;

    //Timer that counts how long a round lasts
    [SerializeField] private float GameActiveDuration = 2f;
    private float GameActiveTimer = 0;

    //How many humans can exist at a time
    [SerializeField] private int MaxHumans = 10;
    private int humanCount = 0;

    public int StartHumanSummon = 30;

    //Score multipliers for the different scare methods
    public const int ScareMultiplier = 2;
    public const int KillMultiplier = 5;
    public const int ApproachMultiplier = 1;

    [SerializeField] private Transform spawnPlayerParent;
    [SerializeField] private Transform spawnHumanParent;

    private Transform[] allPlayerSpawnpoints;
    private Transform[] allHumanSpawnpoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //scoreTMP = scoreTextObject.GetComponent<TextMeshPro>();
        HumanSpawnTimer = HumanSpawnRate;
        
        allHumanSpawnpoints = spawnHumanParent.GetComponentsInChildren<Transform>();
        allPlayerSpawnpoints = spawnPlayerParent.GetComponentsInChildren<Transform>();

        Transform chosenSpawnpoint = allPlayerSpawnpoints[Mathf.FloorToInt(Random.Range(0, allPlayerSpawnpoints.Length - 1))];

        chosenSpawnpoint.position = new Vector3(chosenSpawnpoint.position.x, chosenSpawnpoint.position.y, -5);

        myPlayer = Instantiate(player, chosenSpawnpoint.position, transform.rotation).GetComponent<PlayerMovement>();
        myPlayer.gameManager = this;

        chosenSpawnpoint.gameObject.SetActive(false);

        StartRound();


    }

    private void Awake()
    {
        //Immediately spawn the player upon game start
          
        
    }

    // Update is called once per frame
    void Update()
    {
        
        switch (game_state)
        {
            case GAME.Off:

                

                break;

            //When a round is active for the player to be scaring humans
            case GAME.Active:

                scareScoreText.text =
                "Spookiness: " + myPlayer.approachCount + "\n" +
                "Scariness: " + myPlayer.scareCount + "\n" +
                "Killiness: " + myPlayer.killCount;

                //SPAWN THE HUMAN
                HumanSpawnTimer = Mathf.Max(0, HumanSpawnTimer - Time.deltaTime);
                if (HumanSpawnTimer == 0)
                {
                    if (humanCount < MaxHumans) SpawnHuman();
                    HumanSpawnTimer = HumanSpawnRate;
                }

                //Round Timer
                GameActiveTimer = Mathf.Min(GameActiveDuration, GameActiveTimer + Time.deltaTime);
                if (GameActiveTimer == GameActiveDuration)
                {
                    EndRound();
                }

                break;
        }
    }

    //StartRound(), sets up and starts the game
    public void StartRound()
    {
        scareScoreText.gameObject.SetActive(true);
        ReviewScreen.SetActive(false);
        myPlayer.ResetCount();
        game_state = GAME.Active;
        GameActiveTimer = 0;
        globalLight.intensity = 0.1f;

        for (int i = 0; i < StartHumanSummon; i++)
        {
            SpawnHuman();
        }

    }

    //EndRound(), cleans up and ends the round
    public void EndRound()
    {
        scareScoreText.gameObject.SetActive(false);
        ReviewScreen.SetActive(true);
        UpdateEndText();
        game_state = GAME.Off;
        globalLight.intensity = 1f;

        GameObject[] allHumans = GameObject.FindGameObjectsWithTag("Human");
        int numHumans = allHumans.Length;

        for (int i = 0; i < numHumans; i++)
        {
            Destroy(allHumans[i]);

        }

        humanCount = 0;
    }
    
    //UpdateEndText() updates the text that is displayed after a round
    private void UpdateEndText()
    {

        int killScore = myPlayer.killCount * KillMultiplier;
        int scareScore = myPlayer.scareCount * ScareMultiplier;
        int approachScore = myPlayer.approachCount * ApproachMultiplier;

        int bonus = 0;

        reviewScoreText.text =
            "Spookiness: " + approachScore + "\n" +
            "Scariness: " + scareScore + "\n" +
            "Killiness: " + killScore + 
            (bonus == 0 ? "" : "Bonus: " + bonus);

        int finalScore = killScore + scareScore + approachScore;

        ratingText.text = "Scare-o-meter Rating: " + finalScore;
    }

    //SpawnHuman() creates an instance of a Human in the world
    private void SpawnHuman()
    {
        
        Vector3 offsetVec = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0) * SpawnRange;

        Transform chosenSpawnpoint = allHumanSpawnpoints[Mathf.FloorToInt(Random.Range(0, allHumanSpawnpoints.Length))];

        chosenSpawnpoint.position = new Vector3(chosenSpawnpoint.position.x, chosenSpawnpoint.position.y, -5);

        var newHuman = Instantiate(human, chosenSpawnpoint.position, transform.rotation);
        newHuman.GetComponent<HumanMovement>().gameManager = this; 

        //Sprite
        int choose = Random.Range(0, variations.Length);
        newHuman.GetComponent<Animator>().runtimeAnimatorController = variations[choose];
    }

    //GameActive() checks if the round is active
    public bool GameActive()
    {
        return game_state == GAME.Active;
    }
}
