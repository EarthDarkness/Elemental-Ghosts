using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeGame : ASignal<bool> { }
public class MatchManager : Singleton<MatchManager>
{
    public float timeToUI = 0.5f;
    public float timeToRespawn = 3.0f;
    public TMPro.TextMeshProUGUI countdownUI;
    public List<PlayerData> playerList = new List<PlayerData>();
    // Use this for initialization
    void Start()
    {
        Signals.Get<PlayerKilled>().AddListener(CheckWinCondition);
        StartCoroutine(_StartGame());
    }

    private void OnDestroy()
    {
        Signals.Get<PlayerKilled>().RemoveListener(CheckWinCondition);
    }

    private void CheckWinCondition(PlayerData arg1, PlayerData arg2)
    {
        int count = 0;
        for(int i = 0; i < playerList.Count; i++)
        {
            count += playerList[i].alive ? 1 : 0;
            if (count > 1)
                return;
        }
        int winner = 0;
        // give score
        for (int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].alive)
            {
                winner = i;
                playerList[i].matchScore++;
                break;
            }
        }

        StartCoroutine(_EndGame());

    }

    public IEnumerator _EndGame()
    {
        yield return new WaitForSeconds(timeToUI);
        // Call UI
        yield return new WaitForSeconds(timeToRespawn);
        // Call respawn

        StartCoroutine(_StartGame());
    }

    public IEnumerator _StartGame()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            playerList[i].Ressurect();
        }
        Signals.Get<FreezeGame>().Dispatch(true);
        countdownUI.text = "3";
        yield return new WaitForSeconds(1);
        countdownUI.text = "2";
        yield return new WaitForSeconds(1);
        countdownUI.text = "1";
        yield return new WaitForSeconds(1);
        countdownUI.text = "GO";
        yield return new WaitForSeconds(1);
        countdownUI.text = "";
        Signals.Get<FreezeGame>().Dispatch(false);


    }


    internal void AddPlayer(PlayerData playerData)
    {
        playerList.Add(playerData);
    }
}
