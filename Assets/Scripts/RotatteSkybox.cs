using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatteSkybox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 angle =this.transform.eulerAngles;
		angle+=Vector3.up*Time.deltaTime;
		angle.y=angle.y%360;
		this.transform.eulerAngles= angle;
		
	}
}
