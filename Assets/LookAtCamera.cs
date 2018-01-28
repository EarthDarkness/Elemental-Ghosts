using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {
    
	void Update () {
        transform.LookAt(Camera.main.transform);
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);

    }
}
