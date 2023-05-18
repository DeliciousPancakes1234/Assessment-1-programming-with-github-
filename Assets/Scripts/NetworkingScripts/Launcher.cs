using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Serializable fields
    #endregion

    #region Private fields
    //this is the clients version number.
    string gameVersion = "1";
    #endregion

    #region Monobehaviour callbacks
    private void Awake()
    {
        //Critical line of code 
        //make sure everyones scenes are synched when loadLevel is called
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //on start, try to connect to the master server
    private void Start()
    {
        Connect();
    }
    #endregion

    #region public methods 

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            //attempt to join lobby
            PhotonNetwork.JoinLobby();
        }
        else
        {
            //attemot to connect using server settings, the set your game version
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }
    #endregion

    #region Pun Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Succesfully connected server. Attempting to join a lobby");
        PhotonNetwork.JoinLobby();
    }

    //moniter for disconnecting 
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Failed to connect. OnDisconnected was called woth the reasion {0}", cause);
        SceneManager.LoadScene(0);
    }

    //load the next scene if we succesfully joined a lobby
    public override void OnJoinedLobby()
    {
        PhotonNetwork.LoadLevel("CreateOrJoinARoom");
    }
    #endregion
}
