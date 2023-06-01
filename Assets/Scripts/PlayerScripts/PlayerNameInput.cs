using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    TMP_InputField input;
    private void Start()
    {
        input = GetComponent<TMP_InputField>();
    }
    public void InputName(int playerNumber)
    {
        GameManager.instance.currentPlayers[playerNumber - 1].playerName = input.text;
    }
}
