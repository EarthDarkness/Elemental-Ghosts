using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour{
	public bool _lock = false;
	public List<GameObject> _component = new List<GameObject>();
	GameObject _elementVisual = null;
	public int _elementType = 6;//6 = no element (TODO element type enum)


	/*void Awake() {
		for(int i=0;i<transform.childCount;++i) {
			_component.Add(transform.GetChild(i).gameObject);
		}
	}*/

	void OnValidate() {
		if (_lock)
			return;
		for(int i=transform.childCount;i<_component.Count;++i) {
			//if(_component[i] == null) {
			//	_component.RemoveAt(i);
			//}else {
				Instantiate(_component[i],transform);
			//}
		}
		/*for(int i=0;i<_component.Count;++i) {
			if (transform.childCount > i || _component[i] == null)
				continue;
			_
		}*/
	}

	public TileCell() {

	}



};

public class Board : MonoBehaviour{

	public bool _lock = false;//looks interactivity (true for prefab)
    public int _width   = 20;
    public int _height  = 20;

	float[] _dim = new float[2]{ 1.0f,1.0f};

	GameObject[,] _map;

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

	void PickElement() {
		//GameObject player;//hook

		//PlayerMovement pm = player.GetComponent<PlayerMovement>();

		//int gix = GetTileX(player.transform.position.x);
		//int giy = GetTileX(player.transform.position.z);

		//int ele = _map[gix, giy].GetComponent<TileCell>()._elementType;
		//_map[gix, giy].GetComponent<TileCell>()._elementType = 6;//no element
		////player._element = ele;

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
}



