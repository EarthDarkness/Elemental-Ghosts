﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour{
	public List<CharacterMovement> _players;

	public bool _lock = false;//looks interactivity (true for prefab)
    public int _width   = 20;
    public int _height  = 20;

    public float _thresholdAlign = 0.1f;

    [InspectorReadOnly, SerializeField] // if not serialized this won't be kept
	float[] _dim = new float[2]{ 1.0f,1.0f};

    [InspectorReadOnly, SerializeField] // if not serialized this won't be kept
    GameObject[,] _map;
	void Awake() {
        if(_map == null)
        {
            Debug.LogWarning("map is null, fetching object");
            bool temp = _lock;
            _lock = false;
            Fetch();
            _lock = temp;
        }
		Signals.Get<CharacterCreated>().AddListener(join);
	}
	void Start() {
		Signals.Get<PickupElement>().AddListener(PickElement);
	}
	void Update() {
		for(int i=0;i<_players.Count;++i) {
			if(_players[i].currentDirection == PlayerInput.Direction.Left || _players[i].currentDirection == PlayerInput.Direction.Right) {
				_players[i].aligned = (GetAlignX(_players[i].transform.position.x)< _thresholdAlign);
			}else if(_players[i].currentDirection == PlayerInput.Direction.Top || _players[i].currentDirection == PlayerInput.Direction.Bottom) {
				_players[i].aligned = (GetAlignY(_players[i].transform.position.z)< _thresholdAlign);
			}


		}
	}


	[EasyButtons.Button()]
	void Fetch() {
		if (_lock)
			return;

		float minx = -(_width*_dim[0])/2;
		float miny = -(_height*_dim[1])/2;
		if(_map == null)
			_map = new GameObject[_width,_height];

		for(int j=0;j<_height;++j) {
			GameObject line;
			if(transform.childCount > j) {
				line = transform.GetChild(j).gameObject;
				for(int i=0;i<_width;++i) {
					if(line.transform.childCount > i) {
						_map[i, j] = line.transform.GetChild(i).gameObject;
					}else {
						_map[i,j] = new GameObject();
						_map[i,j].transform.parent = line.transform;
						_map[i, j].transform.position = new Vector3(minx+_dim[0]*i,0.0f,miny+_dim[1]*j);
						_map[i,j].AddComponent<TileCell>();
						_map[i, j].name = "Col "+i.ToString();
					}
				}
			}else {
				line = new GameObject();
				line.name = "Row "+j.ToString();
				line.transform.parent = transform;
				for(int i=0;i<_width;++i) {
					_map[i,j] = new GameObject();
					_map[i,j].transform.parent = line.transform;
					_map[i, j].transform.position = new Vector3(minx+_dim[0]*i,0.0f,miny+_dim[1]*j);
					_map[i,j].AddComponent<TileCell>();
					_map[i, j].name = "Col "+i.ToString();
				}
			}
		}
        Debug.Log(_map);
	}

	[EasyButtons.Button()]
	void Deploy() {
		if (_lock)
			return;
		float minx = -(_width*_dim[0])/2;
		float miny = -(_height*_dim[1])/2;
		_map = new GameObject[_width,_height];
		
		for(int j=0;j<_height;++j) {
			GameObject line = new GameObject();
			line.name = "Row "+j.ToString();
			line.transform.parent = transform;
			for(int i=0;i<_width;++i) {
				_map[i,j] = new GameObject();
				_map[i,j].transform.parent = line.transform;
				_map[i, j].transform.position = new Vector3(minx+_dim[0]*i,0.0f,miny+_dim[1]*j);
				_map[i,j].AddComponent<TileCell>();
				_map[i, j].name = "Col "+i.ToString();
			}
		}

	}
	[EasyButtons.Button()]
	void Reset() {
		if (_lock)
			return;
		if (_map == null)
			return;
		for(int j=0;j<_height;++j) {
			for(int i=0;i<_width;++i) {
				//if (_map[i, j] == null)
				//	continue;
				_map[i,j].GetComponent<TileCell>()._component.Clear();
				DestroyImmediate(_map[i,j]);
				_map[i,j] = null;
			}
		}
		/*for(int i=transform.childCount-1;i>=0;++i) {
			DestroyImmediate(transform.GetChild(i).gameObject);
		}*/
		while(transform.childCount > 0) {
			DestroyImmediate(transform.GetChild(transform.childCount-1).gameObject);
		}

	}

	[EasyButtons.Button()]
	void ToogleLock() {
		_lock = !_lock;
		for(int j=0;j<_height;++j) {
			for(int i=0;i<_width;++i) {
				_map[i, j].GetComponent<TileCell>()._lock = _lock;
			}
		}

	}

	int GetTileX(float x) {
		float minx = -(_width*_dim[0])/2;
		float gx = x - minx;
		return Mathf.RoundToInt(gx);
	}
	int GetTileY(float y) {
		float miny = -(_height*_dim[1])/2;
		float gy = y - miny;
		return Mathf.RoundToInt(gy);
	}
	float GetAlignX(float x) {
		float minx = -(_width*_dim[0])/2;
		float gx = x - minx;
		return Mathf.Abs(gx-Mathf.Round(gx));
	}
	float GetAlignY(float y) {
		float miny = -(_height*_dim[1])/2;
		float gy = y - miny;
		return Mathf.Abs(gy-Mathf.Round(gy));
	}

	void PickElement(ElementBending bend) {
		if (bend.elementType != ElementTable.ElementType.Neutral)
			return;

		int gix = GetTileX(bend.transform.position.x);
		int giy = GetTileX(bend.transform.position.z);

		ElementTable.ElementType ele = (ElementTable.ElementType)_map[gix, giy].GetComponent<TileCell>()._elementType;
		_map[gix, giy].GetComponent<TileCell>()._elementType = 6;//no element
		bend.elementType = ele;

	}
	void DropElement() {
		//GameObject shot;
		//Projectile pj = shot.GetComponent<Projectile>();
		
		//int gix = GetTileX(shot.transform.position.x);
		//int giy = GetTileX(shot.transform.position.z);

		//int nElement;// = pj._elementType;
		//int cElement = _map[gix, giy].GetComponent<TileCell>()._elementType;
		//if((int)ElementTable.weakness[cElement] == nElement) {
		//	_map[gix, giy].GetComponent<TileCell>()._elementType = nElement;
		//}else if((int)ElementTable.weakness[nElement] == cElement) {
		//	//stay same
		//}else {
		//	_map[gix, giy].GetComponent<TileCell>()._elementType = 6;//no element
		//}

	}

	void join(CharacterMovement cm) {
		_players.Add(cm);
	}

}



