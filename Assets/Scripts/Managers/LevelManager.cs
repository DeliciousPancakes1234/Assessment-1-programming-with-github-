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
    public InLevelUI UIManager;

    

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
    }
    //Run a timer between waves for a prep time
    public void StartPrep()
    {
        currentState = GameStates.Prepping;
        timer.StartTimer(timeBetweenWaves);
    }
    //Find and run the current wave until all enemies are dead 
    public void BeginWave()
    {
        currentState = GameStates.InWave;
        waves[currentWave].isActive = true;
        //UI Update
    }
    //Track player deaths and run game over 

    //Track waves completed and run victory
    public void EndWave()
    {
        currentWave++;
        if(currentWave < waves.Length)
        {
            StartPrep();
        }
        else
        {
            currentState = GameStates.Won;
        }
    }
    //process score and update the other scripts 
    public void PlayerDeath(int playerNumber)
    {

    }
}
