using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : Singleton<MatchManager>
{

    public List<PlayerData> playerList = new List<PlayerData>();
    // Use this for initialization
    void Start()
    {
        Signals.Get<PlayerKilled>().AddListener(CheckWinCondition);
    }

    private void CheckWinCondition(PlayerData arg1, PlayerData arg2)
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
