﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
	Board _map = null;

	int _maxElements = 8;
	public static int _count = 0;

	float _baseDelay = 4.0f;
	float _cooldown = 0.0f;
	// Use this for initialization
	void Start () {
		_map = transform.GetComponent<Board>();
	}
	
	// Update is called once per frame
	void Update () {
		if (_count >= _maxElements)
			return;
		if(_cooldown <= 0.0f) {
			float rx = Random.value*((float)_map._width-0.5f);
			float ry = Random.value*((float)_map._height-0.5f);

			float re = Random.value*4.5f;
			if (_map.SpawnElement(Mathf.FloorToInt(rx), Mathf.FloorToInt(ry), Mathf.FloorToInt(re)))
				++_count;

			_cooldown = _baseDelay;
		}
		_cooldown -= Time.deltaTime;
	}
}
