using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCameraTarget : MonoBehaviour
{

    public DynamicCamera dynamicCamera;
    bool active = false;
    public void SetActive(bool active)
    {
        if (dynamicCamera == null)
        {
            dynamicCamera = FindObjectOfType<DynamicCamera>();
        }
        if (dynamicCamera == null)
            return;
        if (active && !this.active)
        {
            dynamicCamera.trackList.Add(transform);
        }
        else if (!active && this.active)
        {
            dynamicCamera.trackList.Remove(transform);
        }
        this.active = active;
    }


    public void OnEnable()
    {
        SetActive(true);
    }

    public void OnDisable()
    {
        SetActive(false);
    }
}
