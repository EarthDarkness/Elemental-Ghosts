using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour
{

    public string soundName;
    Button button;

    // Use this for initialization
    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => AudioManager.Instance.PlaySound(soundName));
        }


    }

    public void Play()
    {
        AudioManager.Instance.PlaySound(soundName);
    }

}
