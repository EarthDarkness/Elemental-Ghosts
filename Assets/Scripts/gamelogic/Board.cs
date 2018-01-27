using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour{
	public List<GameObject> _component = new List<GameObject>();
	GameObject _element = null;

	/*void Awake() {
		for(int i=0;i<transform.childCount;++i) {
			_component.Add(transform.GetChild(i).gameObject);
		}
	}*/

	void OnValidate() {
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


    public int _width   = 20;
    public int _height  = 20;

	float[] _dim = new float[2]{ 1.0f,1.0f};

	GameObject[,] _map;

	[EasyButtons.Button()]
	void Deploy() {
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


}



