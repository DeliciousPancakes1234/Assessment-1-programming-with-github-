using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class EnemyHealthFromSpawner : HealthWithUI
{
    public bool isOnline;
    public WaveManager manager;
    public int lastDamagedBy;
    PhotonView view;

    public override void Die()
    {
        if (isOnline)
        {
            view.RPC("AllOnlineEnemiesDead", RpcTarget.All);
        }
        else
        {
            LevelManager.instance.IncreaseScore(lastDamagedBy - 1);
            manager.KillEnemy(gameObject);
        }
        base.Die();
        Destroy(gameObject, 0.5f);
    }

    [PunRPC]
    void AllOnlineEnemiesDead()//Makes sure that dead enemies disappear for all players 
    {
        if (PhotonNetwork.IsMasterClient)
        {
            OnlineLevelManager.instance.IncreaseScore(lastDamagedBy - 1);
            manager.KillEnemy(gameObject);
        }
        Destroy(gameObject);
    }
}
