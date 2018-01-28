using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    Sound()
    {
        volume = 1;
        minPitch = 1;
        maxPitch = 1;
    }
    public string name;
    public AudioClip clip;
    public AudioMixerGroup mixer;

    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.1f, 2f)]
    public float minPitch = 1f;
    [Range(0.1f, 2f)]
    public float maxPitch = 1f;
    public int priority = 128;
    public bool reverse;

    public bool loop = false;
    public bool isMusic = false;
    [HideInInspector]
    public AudioSource lastSource;

   
    public void Play(AudioSource source)
    {
        source.loop = loop;
        source.clip = clip;
        source.volume = volume;
        if (minPitch == maxPitch)
            source.pitch = minPitch;
        else
            source.pitch = Random.Range(minPitch, maxPitch);
        if (reverse)
        {
            source.timeSamples = clip.samples - 1;
            source.pitch *= -1;
        }
        else
        {
            source.timeSamples = 0;

        }
        source.priority = priority;

        source.Play();
        lastSource = source;
        source.outputAudioMixerGroup = mixer;

    }

}

public class AudioManager : Singleton<AudioManager>
{

   

    [HideInInspector]
    public AudioSource[] poolSounds;
    [Header("Set up Pool")]
    public int sizePoolSounds;
    int lastUsedSound = 0;

    [HideInInspector]
    public AudioSource[] poolMusic;
    public int sizePoolMusic;
    int lastUsedMusic = 0;

    [Header("Set up Sounds")]
    [SerializeField]
    Sound[] sounds;


    [Header("Setup Mixer")]
    public AudioMixer mixer;

    public void setMusicVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);

    }

    public void setSoundVolume(float volume)
    {
        mixer.SetFloat("SoundVolume", volume);
        PlayerPrefs.SetFloat("SoundVolume", volume);
    }


    override public void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        if (poolSounds == null)
        {
            poolSounds = new AudioSource[sizePoolSounds];
        }
        else if (poolSounds.Length == 0)
        {
            poolSounds = new AudioSource[sizePoolSounds];
        }

        for (int i = 0; i < sizePoolSounds; i++)
        {
            if (poolSounds[i] == null)
            {
                GameObject _go = new GameObject("Sound_" + i);
                _go.transform.SetParent(this.transform, false);
                poolSounds[i] = _go.AddComponent<AudioSource>();
            }

        }

        if (poolMusic == null)
        {
            poolMusic = new AudioSource[sizePoolMusic];
        }
        else if (poolMusic.Length == 0)
        {
            poolMusic = new AudioSource[sizePoolMusic];
        }


        for (int i = 0; i < sizePoolMusic; i++)
        {
            if (poolMusic[i] == null)
            {
                GameObject _go = new GameObject("Music_" + i);
                _go.transform.SetParent(this.transform, false);
                poolMusic[i] = _go.AddComponent<AudioSource>();
            }

        }


        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            setMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        }
        else
        {
            setMusicVolume(0);
        }

        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            setSoundVolume(PlayerPrefs.GetFloat("SoundVolume"));
        }
        else
        {
            setSoundVolume(0);
        }

    }

    void Start()
    {


    }

    public void PlaySound(string _name)
    {
        PlaySound(_name, 0);
    }

    public bool PlaySound(string _name, float fadeInDuration, float pitch = 1.0f, bool onlyPlayOne = false)
    {
        Sound soundToPlay = null;
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                soundToPlay = sounds[i];
                break;
            }
        }
        if (soundToPlay == null)
        {
            Debug.LogWarning("No " + _name + " found on AudioManager");
            return false;
        }

        if (onlyPlayOne)
        {
            if (soundToPlay.lastSource != null)
            {
                if (soundToPlay.lastSource.isPlaying || soundToPlay.lastSource.clip.name == _name)
                    return false;
            }

        }

        int c = 0;

        if (soundToPlay.isMusic)
        {
            for (int i = 0; i < sizePoolMusic; i++)
            {
                c = i + lastUsedMusic;
                c %= sizePoolSounds;
                if (poolMusic[c].priority <= soundToPlay.priority || !poolMusic[c].isPlaying) // only play the desired clip in the source if the sound playing is not as importart, has the same important or if its not playing
                {
                    soundToPlay.Play(poolMusic[c]);
                    if (fadeInDuration != 0)
                    {
                        poolMusic[c].volume = 0;
                        poolMusic[c].Fade(0, soundToPlay.volume, fadeInDuration);
                    }

                    poolMusic[c].pitch *= pitch;
                    lastUsedMusic = c;
                    lastUsedMusic++;
                    if (lastUsedMusic == sizePoolMusic)
                        lastUsedMusic = 0;

                    //Debug.Log("Music " + soundToPlay.name +  " playing on " + c);
                    break;
                }
            }

        }
        else
        {
            for (int i = 0; i < sizePoolSounds; i++)
            {
                c = i + lastUsedSound;
                c %= sizePoolSounds;
                if (poolSounds[c].priority <= soundToPlay.priority || !poolSounds[c].isPlaying) // only play the desired clip in the source if the sound playing is not as importart, has the same important or if its not playing
                {
                    soundToPlay.Play(poolSounds[c]);
                    if (fadeInDuration != 0)
                    {
                        poolSounds[c].volume = 0;
                        poolSounds[c].Fade(0, soundToPlay.volume, fadeInDuration);
                    }
                    poolSounds[c].pitch *= pitch;
                    lastUsedSound = c;
                    lastUsedSound++;
                    if (lastUsedSound == sizePoolSounds)
                        lastUsedSound = 0;

                   // Debug.Log("Sound " + soundToPlay.name + " playing on " + c);

                    break;
                }
            }
        }

        return true;


    }

    public void PlaySoundWithFadeIn(string _name, float fadeInDuration, float pitch = 1.0f)
    {
        PlaySound(_name, pitch, fadeInDuration);
    }

    public void StopSoundWithFade(string name, float fadeDuration)
    {
        string _name = "";
        for(int i = 0; i < sounds.Length; i++)
        {
            if(sounds[i].name == name)
            {
                _name = sounds[i].clip.name;
                break;
            }
        }

        if (_name == "")
            return;

        for (int i = 0; i < poolSounds.Length; i++)
        {
            if (poolSounds[i].clip != null && poolSounds[i].isPlaying)
            {
                if (poolSounds[i].clip.name == _name)
                {
                    poolSounds[i].Fade(0, fadeDuration,true);
                    return;
                }

            }
        }

        for (int i = 0; i < poolMusic.Length; i++)
        {
            if (poolMusic[i].clip != null)
            {
                if (poolMusic[i].clip.name == _name && poolMusic[i].isPlaying)
                {
                    poolMusic[i].Fade(0, fadeDuration,true);
                    return;
                }

            }
        }

    }

    public void StopSound(string _name)
    {
        for (int i = 0; i < poolSounds.Length; i++)
        {
            if (poolSounds[i].isPlaying)
            {
                if (poolSounds[i].clip.name == _name)
                {
                    poolSounds[i].Stop();
                    return;
                }

            }
        }

        for (int i = 0; i < poolMusic.Length; i++)
        {
            if (poolMusic[i].isPlaying)
            {
                if (poolMusic[i].clip.name == _name)
                {
                    poolMusic[i].Stop();
                    return;
                }

            }
        }
    }

    [System.Obsolete("It seems that this method is not working. Need refactoring")]
    public bool IsPlaying(string _name)
    {
        for (int i = 0; i < poolSounds.Length; i++)
        {
            if (poolSounds[i].isPlaying)
            {
                if (poolSounds[i].clip.name == _name)
                {
                    return true;
                }
            }
        }

        for (int i = 0; i < poolMusic.Length; i++)
        {
            if (poolMusic[i].isPlaying)
            {
                if (poolMusic[i].clip.name == _name)
                {
                    return true;
                }
            }
        }

        return false;
    }

}
