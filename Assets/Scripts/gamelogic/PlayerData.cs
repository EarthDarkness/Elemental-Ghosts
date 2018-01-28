﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKilled : ASignal<PlayerData, PlayerData> { }
public class PlayerData : MonoBehaviour
{

    public enum PlayerId
    {
        Player_1,
        Player_2,
        Player_3,
        Player_4,
        Player_5,
        Player_6,
        Player_7,
        Player_8
    }

    public PlayerId playerId;
    public float timeDeath = 0.2f;
    public int killScore = 0;
    public int roundScore = 0;

    public void Start()
    {
        if (PersisterPlayerInputInfo.players != null)
        {
            if (!PersisterPlayerInputInfo.players.ContainsKey(playerId))
            {
                Destroy(gameObject);
                return;
            }
            GetComponent<PlayerInput>().joystickId = PersisterPlayerInputInfo.players[playerId];
        }
        Signals.Get<PlayerKilled>().AddListener(CheckDeath);
    }

    private void CheckDeath(PlayerData killer, PlayerData vitimn)
    {
        if (killer == this)
        {
            killScore++;
        }
    }

    public void Die(PlayerData killer)
    {
        Signals.Get<PlayerKilled>().Dispatch(killer, this);
        // Animate
        // Particles
        StartCoroutine(_WaitToDie());
    }

    public IEnumerator _WaitToDie()
    {
        yield return new WaitForSeconds(timeDeath);
        gameObject.SetActive(false);
    }
}
