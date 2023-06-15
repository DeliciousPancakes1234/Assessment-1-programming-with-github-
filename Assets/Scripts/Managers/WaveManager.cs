using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
/// <summary>
/// Managing spawning of enemies for a wave, as well as how long the wave should last 
/// this works alongside the level manager
/// </summary>

public class WaveManager : MonoBehaviour
{
    [Header("Enemyinfo")]
    public GameObject[] enemyTypes;
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public int maxNumberAtOnce;
    public int maxNumberOverall;

    public bool isActive;
    public bool isComplete;
    public bool isOnline;

    public float timeBetweenSpawns;
    float timer;

    List<GameObject> enemiesSpawned = new List<GameObject>();
    List<GameObject> currentEnemiesSpawned = new List<GameObject>();

    [HideInInspector]
    public int killed;

    public UnityEvent EndWave;

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            timer += Time.deltaTime;
            if(timer > timeBetweenSpawns)
            {
                AttemptToSpawn();
                timer = 0;
            }
        }
    }
    void AttemptToSpawn()
    {
        //pick enemy type
        int type = Random.Range(0, enemyTypes.Length);
        //pick the position
        int pos = Random.Range(0, spawnPoints.Length);

        //check if you can spawn
        if(enemiesSpawned.Count < maxNumberOverall && currentEnemiesSpawned.Count < maxNumberAtOnce)
        {
            if (isOnline)
            {
                GameObject currentEnemy = PhotonNetwork.Instantiate(enemyTypes[type].name, spawnPoints[pos].position, Quaternion.identity);
                currentEnemy.GetComponent<EnemyHealthFromSpawner>().manager = this;
                enemiesSpawned.Add(currentEnemy);
                currentEnemiesSpawned.Add(currentEnemy);
            }
            else
            {
                GameObject currentEnemy = Instantiate(enemyTypes[type], spawnPoints[pos].position, Quaternion.identity);
                currentEnemy.GetComponent<EnemyHealthFromSpawner>().manager = this;
                enemiesSpawned.Add(currentEnemy);
                currentEnemiesSpawned.Add(currentEnemy);
            }
            
        }
        //spawn the enemy

        //update its variables
    }
    public void KillEnemy(GameObject enemy)
    {
        killed++;
        currentEnemiesSpawned.Remove(enemy);
        if(currentEnemiesSpawned.Count == 0 && enemiesSpawned.Count >= maxNumberOverall)
        {
            isActive = false;
            isComplete = true;
            EndWave.Invoke();
        }
    }
}
