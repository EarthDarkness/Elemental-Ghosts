using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInstancer : MonoBehaviour {

    public GameObject Prefab;
    GameObject temp;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    public void createParticles(Transform t)
    {
        Destroy(temp);
        temp = GameObject.Instantiate(Prefab);
        temp.transform.SetParent(t);
        temp.transform.position = t.position;
    }
}
