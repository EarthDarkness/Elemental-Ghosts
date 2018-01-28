using System;
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
    public int matchScore = 0;
    public bool alive = true;
    private Animator animator;

    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (PersisterPlayerInputInfo.players != null)
        {
            if (!PersisterPlayerInputInfo.players.ContainsKey(playerId))
            {
                Destroy(gameObject);
                return;
            }
            GetComponent<PlayerInput>().joystickId = PersisterPlayerInputInfo.players[playerId];
        }
        alive = true;
        MatchManager.Instance.AddPlayer(this);
        Signals.Get<PlayerKilled>().AddListener(CheckDeath);
    }

    private void OnDestroy()
    {
        Signals.Get<PlayerKilled>().RemoveListener(CheckDeath);
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
        AudioManager.Instance.PlaySound("Death");
        alive = false;
        Signals.Get<PlayerKilled>().Dispatch(killer, this);
        // Animate
        animator.SetTrigger("Die");
        // Particles
        StartCoroutine(_WaitToDie());
    }

    public IEnumerator _WaitToDie()
    {
        yield return new WaitForSeconds(timeDeath);
        gameObject.SetActive(false);
    }

    internal void Ressurect()
    {
        gameObject.SetActive(true);
        alive = true;
    }
}
