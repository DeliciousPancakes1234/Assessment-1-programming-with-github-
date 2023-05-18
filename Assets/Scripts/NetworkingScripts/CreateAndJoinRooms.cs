using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    #region Fields Section
    public TMP_InputField joinRoomName;
    public TMP_InputField createRoomName;
    public TMP_Text errorLog;
    public byte maxPlayersPerRoom = 2;
    #endregion

    #region Public functions

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomName.text);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoomName.text, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
    #endregion

    #region Pun callbacks

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        string errorMessage = "Failed to join room. Error:" + message;
        Debug.Log(errorMessage);
        errorLog.text = errorMessage;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LoadLevel("WaitingForPlayers");
    }
    #endregion

}
