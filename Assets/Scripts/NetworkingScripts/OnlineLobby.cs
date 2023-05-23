using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class OnlineLobby : MonoBehaviourPunCallbacks
{
    public bool [] playersReady;

    public PhotonView view;

    public TMP_Text roomName;
    public TMP_Text messages;
    public TMP_Text numberOfPlayers;
    public TMP_InputField playerName;

    public string levelName;

    // Start is called before the first frame update
    void Start()
    {
        playersReady = new bool[PhotonNetwork.CurrentRoom.MaxPlayers];
        PhotonNetwork.LocalPlayer.NickName = " Player" + PhotonNetwork.LocalPlayer.ActorNumber;
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;

        numberOfPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public void UpdateName()
    {
        PhotonNetwork.LocalPlayer.NickName = playerName.text;
    }

    public void LoadLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            messages.text = "You are not the host, you can't do that";
        }
    }
    [PunRPC]
    public void ReadyPlayer(int playerNumber, bool isReady)
    {
        playersReady[playerNumber - 1] = isReady;
    }
    public void RunReadyPlayer(bool isReady)
    {
        view.RPC("ReadyPlayer", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, isReady);
    }
}
