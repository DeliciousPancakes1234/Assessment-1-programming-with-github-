using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages game state and level, plays appropriate waves and manages players scoring and respawning
/// Treat this like the information hub for your level
/// </summary>
/// 
public class LevelManager : MonoBehaviour
{
    #region Singleton
    public static LevelManager instance;

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

    //list of player prefabs
    [Header("Players")]
    public PlayerData[] players;
    public Transform[] playerSpawns;

    //list of spawn points 

    //list of waves 
    [Header("Level Settings")]
    public WaveManager[] waves;
    public int currentWave = 0;

    public Timer timer;

    public float timeBetweenWaves;

    public enum GameStates { Prepping, InWave, Paused, Won, Lost}
    public GameStates currentState;

    [Header("Attached Components and Scripts")]
    public InLevelUIManager UIManager;

    

    // Start is called before the first frame update
    void Start()
    {
        timer.StartTimer(5f);
        currentState = GameStates.Prepping;
    }

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
    public void StartPrep()
    {
        currentState = GameStates.Prepping;
        timer.StartTimer(timeBetweenWaves);

        foreach(PlayerData player in players)
        {
            if (player.GetComponent<Health>().isDead)
            {
                SpawnPlayerOnWaveEnd(player.gameObject);
            }
            else
            {
                player.wavesSurvived++;
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
    //Track player deaths and run game over 

    //Track waves completed and run victory
    public void EndWave()
    {
        if(currentWave < waves.Length)
        {
            currentWave++;
            StartPrep();
        }
        else
        {
            currentState = GameStates.Won;
            UIManager.EndGameUI();
        }
    }
    //process score and update the other scripts 
    public void PlayerDeath(int playerNumber)
    {
        //update player score 
        players[playerNumber].deaths++;

        //Get a game object reference to the player 
        GameObject currentPlayer = players[playerNumber -1].gameObject;

        //Deactivate components
        currentPlayer.GetComponent<CharacterController>().enabled = false;
        currentPlayer.GetComponent<Collider>().enabled = false;
        currentPlayer.GetComponent<Health>().enabled = false;
        currentPlayer.GetComponent<CharacterMovement>().enabled = false;
        currentPlayer.GetComponent<PlayerAttacks>().meleeCollider.SetActive(false);
        currentPlayer.GetComponent<PlayerAttacks>().enabled = false;

        bool anyAlive = false;
        foreach(PlayerData player in players)
        {
            if(player.GetComponent<PlayerHealth>().isDead == false)
            {
                anyAlive = true;
            }
        }
        if(anyAlive == false)
        {
            currentState = GameStates.Lost;
            UIManager.EndGameUI();
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
}
