using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// Manages game state and level, plays appropriate waves and manages players scoring and respawning
/// Treat this like the information hub for your level
/// </summary>

public class OnlineLevelManager : MonoBehaviour, IOnEventCallback
{
    #region Singleton
    public static OnlineLevelManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    #region variables
    //list of player prefabs
    [Header("Players")]
    public GameObject[] players;
    public GameObject[] playerPrefabs;
    public Transform[] playerSpawns;

    //list of waves 
    [Header("Level Settings")]
    public WaveManager[] waves;
    public int currentWave = 0;

    public OnlineTimer timer;

    public float timeBetweenWaves;

    public enum GameStates { Prepping, InWave, Paused, Won, Lost}
    public GameStates currentState;

    [Header("Attached Components and Scripts")]
    public InLevelUIManager UIManager;
    PhotonView view;

    //Event data 

    private const byte CHANGE_STATE = 2;
    #endregion

    #region game set up
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        timer.StartTimer(5f);
        currentState = GameStates.Prepping;

        Invoke("SpawnPlayerAtStart",1);
    }
    void SpawnPlayerAtStart()
    {
        //Spawn player 
        int a = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        GameObject player  = PhotonNetwork.Instantiate(playerPrefabs[a].name, playerSpawns[a].position, Quaternion.identity).gameObject;
        if (player == null) Debug.Log("No Player reference");
        Invoke("CallAddPlayerToList", 1);

        //assign camera in scene
        //player.GetComponentInChildren<Camera>().tag = "MainCamera";
    }

    void CallAddPlayerToList()
    {
        view.RPC("AddPlayerToList", RpcTarget.All);
    }

    [PunRPC]
    void AddPlayerToList() //Find all instances of players and add them to the array 
    {
        GameObject[] playersFound = GameObject.FindGameObjectsWithTag("Player");//find objects 
        foreach(GameObject player in playersFound)//itterate through each object
        {
            int playerNum = player.GetComponent<PhotonView>().OwnerActorNr - 1;//get the objects player number
            players[playerNum] = player;//assign the object to the correct slot 
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        //UI update
        if(currentState == GameStates.Prepping)
        {
            UIManager.UpdateUI();
        }
    }
    //Run a timer between waves for a prep time
    #region ManageGameStates
    public void StartPrep()
    {
        currentState = GameStates.Prepping;
        timer.StartTimer(timeBetweenWaves);

        foreach(GameObject player in players)
        {
            if (player.GetComponent<Health>().isDead)
            {
                SpawnPlayerOnWaveEnd(player);
            }
            else
            {
                int playerNum = player.GetComponent<PlayerNumber>().playerNumber -1;
                GameManager.instance.currentPlayers[playerNum].wavesSurvived++;
                Debug.Log(GameManager.instance.currentPlayers[playerNum].wavesSurvived);
            }
        }
    }
    //Find and run the current wave until all enemies are dead 
    public void BeginWave()
    {
        currentState = GameStates.InWave;
        waves[currentWave].isActive = true;
        UIManager.UpdateUI();
    }

    //Track waves completed and run victory
    public void EndWave()
    {
        currentWave++;
        if (currentWave < waves.Length)
        {
            StartPrep();
        }
        else
        {
            currentState = GameStates.Won;
            foreach (GameObject player in players)
            {
                if (!player.GetComponent<Health>().isDead)
                {
                    int playerNum = player.GetComponent<PlayerNumber>().playerNumber - 1;
                    GameManager.instance.currentPlayers[playerNum].wavesSurvived++;
                }
            }
            UIManager.EndGameUI();
            Invoke("SaveResultsAndLoadScene", 5);
        }
    }
    //process score and update the other scripts 
    public void PlayerDeath(int playerNumber)
    {
        //update player score 
        GameManager.instance.currentPlayers[playerNumber - 1].deaths++;

        //Get a game object reference to the player 
        GameObject currentPlayer = players[playerNumber -1];

        //Deactivate components
        currentPlayer.GetComponent<CharacterController>().enabled = false;
        currentPlayer.GetComponent<Collider>().enabled = false;
        currentPlayer.GetComponent<Health>().enabled = false;
        currentPlayer.GetComponent<CharacterMovement>().enabled = false;
        currentPlayer.GetComponent<PlayerAttacks>().meleeCollider.SetActive(false);
        currentPlayer.GetComponent<PlayerAttacks>().enabled = false;

        //Check to see if all players are dead. if so end the game
        bool anyAlive = false;
        foreach(GameObject player in players)
        {
            if(player.GetComponent<PlayerHealth>().isDead == false)
            {
                anyAlive = true;
            }
        }
        if(anyAlive == false)
        {
            Debug.Log("No players alive");
            currentState = GameStates.Lost;
            UIManager.EndGameUI();
            Invoke("SaveResultsAndLoadScene",5);
        }
    }
    
    void SpawnPlayerOnWaveEnd(GameObject player)
    {
        //Deactivate components
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<Collider>().enabled = true;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.enabled = true;
        playerHealth.currentHealth = playerHealth.maxHealth * 0.75f;
        playerHealth.UpdateUI();



        player.GetComponent<CharacterMovement>().enabled = true;
        player.GetComponent<PlayerAttacks>().enabled = true;
        player.GetComponentInChildren<Animator>().SetBool("Dead", false);

    }
    void SaveResultsAndLoadScene()
    {
        Debug.Log("Loading the end of the game");
        GameManager.instance.FillTempList();
        GameManager.instance.FillSaveData();
        SceneManager.LoadScene("Results");
    }

    public void IncreaseScore(int playerNumber)
    {
        GameManager.instance.currentPlayers[playerNumber].kills++;
        UIManager.UpdateUI();
        Debug.Log(GameManager.instance.currentPlayers[playerNumber].kills);
    }
    #endregion

    #region Pun event methods
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == CHANGE_STATE)
        {
            object[] data = (object[])photonEvent.CustomData;
            currentState = (GameStates)data[0];
            UIManager.UpdateUI();//Change1 
            PhotonNetwork.RaiseEvent(CHANGE_STATE, data, RaiseEventOptions.Default, SendOptions.SendUnreliable);//Change2
        }
    }
    #endregion
}
