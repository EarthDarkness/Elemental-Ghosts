using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

    public AudioInfo[] info;
    public Dictionary<string, AudioInfo> infoDictionary = new Dictionary<string, AudioInfo>();

    // Use this for initialization
    new void Awake()
    {
        base.Awake();
        for (int i = 0; i < info.Length; i++)
        {
            infoDictionary.Add(info[i].Name, info[i]);
        }
    }


    public static void PlaySound(string id, bool Loop = false, float pitch = 1f, bool forceReplay = true)
    {
        Instance.infoDictionary[id].Play(Loop, pitch, forceReplay);
    }

    public static void StopSound(string id)
    {
        Instance.infoDictionary[id].Stop();
    }


}
[System.Serializable]
public class AudioInfo
{
    public AudioSource[] Source;
    public string Name;
    public void Play(bool loop, float pitch, bool forceReplay)
    {

        for (int i = 0; i < Source.Length; i++)
        {
            if (Source[i].isPlaying)
            {
                if (forceReplay)
                {
                    Source[i].Stop();
                }
                else
                {
                    return;
                }
            }
        }

        int id = Random.Range(0, Source.Length);
        Source[id].loop = loop;
        Source[id].pitch = pitch;
        Source[id].Play();
    }
    public void Stop()
    {
        for (int i = 0; i < Source.Length; i++)
        {
            Source[i].Stop();
        }
    }
}