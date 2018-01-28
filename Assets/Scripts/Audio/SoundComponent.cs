using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundComponent : MonoBehaviour {

    [MinMaxRange()]
    public MinMaxRange pitch;
    AudioSource source;
    public bool PlayOnAwake;
    void Awake()
    {
        source = GetComponent<AudioSource>();
        if (PlayOnAwake)
            Play();
    }
	public void Play()
    {
        source.pitch = pitch.GetRandomValue();
        source.Play();
    }
}
