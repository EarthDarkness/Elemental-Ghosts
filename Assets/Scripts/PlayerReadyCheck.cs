using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniversalNetworkInput;

public static class PersisterPlayerInputInfo
{
    public static Dictionary<PlayerData.PlayerId, int> players;
}

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
        PersisterPlayerInputInfo.players = new Dictionary<PlayerData.PlayerId, int>();
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
            SceneManager.LoadScene("Game");
            for (int i = 0; i < players.Length; i++)
            {
                players[i].Save();
            }
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
    public int joystickID;
    public Animator CrystalController;
    public TMPro.TextMeshProUGUI text;
    public Sprite Grey, Normal;
    public Image CrystalBase;
    PlayerData.PlayerId ID;
    private VirtualInput vi;

    public void Joined(int inputKey)
    {
        this.joystickID = inputKey;
        hasClicked = true;
        text.text = "Player " + ((int)ID + 1) + " has joined the game";
        CrystalBase.sprite = Normal;
        CrystalController.SetTrigger("Joined");
    }
    public void Ready()
    {
        isReady = true;
        text.text = "Player " + ((int)ID + 1) + " is ready";
        CrystalController.SetTrigger("Ready");
        PlayerReadyCheck.instance.ReadyCheck();
    }
    public void SetId(int id)
    {
        this.ID = (PlayerData.PlayerId)id;
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

        if (joystickID == -1)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Back();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Ready();
            }
        }
        else
        {
            UNInput.GetInputReference(joystickID, out vi);
            if (!vi.connected)
            {
                Unready();
                Unjoin();
                return;
            }

            if (UNInput.GetButtonDown(joystickID, "Back"))
            {
                Back();
            }

            if (UNInput.GetButtonDown(joystickID, "Action"))
            {
                Ready();
            }
        }
    }

    public void Unready()
    {
        text.text = "Player " + ((int)ID + 1) + " has joined the game";
        isReady = false;
        CrystalController.SetTrigger("Unready");
    }

    public void Unjoin()
    {
        PlayerReadyCheck.instance.Remove(joystickID);

        hasClicked = false;
        CrystalController.SetTrigger("Unjoin");
        text.text = "Waiting for a player";
        CrystalBase.sprite = Grey;
    }

    public void Back()
    {
        if (isReady)
        {
            Unready();
        }
        else
        {
            Unjoin();
        }

    }

    public void Save()
    {
        if (hasClicked)
        {
            PersisterPlayerInputInfo.players.Add(ID, joystickID);
        }
    }



}