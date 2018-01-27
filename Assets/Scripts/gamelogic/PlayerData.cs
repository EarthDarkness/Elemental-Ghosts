using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour {

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
    public int score = 0;

    public void Die()
    {
        // Animate
        // Particles
        // 
        StartCoroutine(_WaitToDie());
    }

    public IEnumerator _WaitToDie()
    {
        yield return new WaitForSeconds(timeDeath);
        gameObject.SetActive(false);
    }
}
