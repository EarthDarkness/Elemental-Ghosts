using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniversalNetworkInput;


public class PlayerReadyCheck : MonoBehaviour
{


    public PlayerInfo[] players;

    public int waitingID = 0;
    public List<int> remainingInputs = new List<int>();
    public bool keyboardAvailable = true;
    public static PlayerReadyCheck instance;
    // Use this for initialization
    void Start()
    {
        instance = this;
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetId(i);
            remainingInputs.Add(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].Update();
        }

        for (int i = 0; i < remainingInputs.Count; i++)
        {
            if (UNInput.GetButtonDown(remainingInputs[i], "Action"))
            {
                ClickedJoined(remainingInputs[i]);
                remainingInputs.Remove(remainingInputs[i]);
            }
        }
        if (keyboardAvailable && Input.GetKeyDown(KeyCode.Space))
        {
            ClickedJoined(-1);
            keyboardAvailable = false;
        }

    }
    public void ClickedJoined(int inputKey)
    {
        int nextId = 0;
        for (; nextId < players.Length; nextId++)
        {
            if (!players[nextId].GetJoined())
                break;
        }
        if (nextId < players.Length)
        {
            players[nextId].Joined(inputKey);
        }
    }
    public void ReadyCheck()
    {
        int joinedAndReady = 0;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetJoined())
            {
                if (players[i].GetReady())
                    joinedAndReady++;
                else return;
            }

        }
        if (joinedAndReady >= 2)
        {
            // DO THE THING THAT LOADS THE GAME
        }
    }

    internal void Remove(int inputKey)
    {
        if (inputKey == -1)
            keyboardAvailable = true;
        else
            remainingInputs.Add(inputKey);
    }
}
[System.Serializable]
public class PlayerInfo
{
    bool isReady = false;
    bool hasClicked = false;
    public int inputKey;
    public Animator CrystalController;
    public TMPro.TextMeshProUGUI text;
    public Color col;
    public Image CrystalBase;
    int ID = -1;
    public void Joined(int inputKey)
    {
        this.inputKey = inputKey;
        hasClicked = true;
        text.text = "Player " + (ID + 1) + " has joined the game";
        CrystalBase.color = col;
        CrystalController.SetTrigger("Joined");
    }
    public void Ready()
    {
        isReady = true;
        text.text = "Player " + (ID + 1) + " is ready";
        CrystalController.SetTrigger("Ready");
        PlayerReadyCheck.instance.ReadyCheck();
    }
    public void SetId(int id)
    {
        this.ID = id;
    }
    public bool GetJoined()
    {
        return hasClicked;
    }
    public bool GetReady()
    {
        return isReady;
    }

    public void Update()
    {
        if (!hasClicked)
            return;

        if (inputKey == -1)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Unjoin();
            }
        }
        else
        {
            if (UNInput.GetButtonDown(inputKey, "Back"))
            {
                Unjoin();
            }
        }
    }

    public void Unjoin()
    {
        PlayerReadyCheck.instance.Remove(inputKey);

        hasClicked = false;
        //Resetar o rest ????
    }


}