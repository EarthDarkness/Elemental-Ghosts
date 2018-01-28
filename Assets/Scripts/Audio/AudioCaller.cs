using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCaller : MonoBehaviour {

    AudioManager gAudio;
	// Use this for initialization
	void Start () {
        gAudio = AudioManager.Instance;
        enabled = false;
    }
	

    public void PlaySound(string tag)
    {
        gAudio.PlaySound(tag);
    }
}
