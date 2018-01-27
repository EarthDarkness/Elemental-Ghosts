using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracesController : MonoBehaviour {

	public List<GameObject> traces = new List<GameObject>();
	float timer=0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(traces.Count>0)
		{
			timer+=Time.deltaTime;
			if(timer>0.25f)
			{
				timer=0f;
				int id = Random.Range(0,traces.Count);
				traces[id].SetActive(true);
				traces.RemoveAt(id);
			}
		}
	}
}
