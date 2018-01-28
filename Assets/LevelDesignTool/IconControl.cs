using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconControl : MonoBehaviour {
	public GameObject _mule = null;
	public Board _container = null;
	public List<GameObject> _props;
	public int _id = -1;
	// Use this for initialization

	public Vector3 _areaPos = Vector3.zero;
	public GameObject _area = null;
	public GameObject _iconPrefab = null;
	
	void SetLayout(int x, int y, float px, float py, int sign, Transform p) {
		GameObject go = Instantiate(_iconPrefab, p);
		go.GetComponent<Icon>().Setup(x,y,sign);
		go.GetComponent<Icon>()._ctrl = this;
		go.transform.position = new Vector3(px, 3.1f, py);
	}

	public void Deploy(int w, int h, float xdim, float ydim) {
		_mule = new GameObject();

		float minx = -(w*xdim)/2;
		float miny = -(h*ydim)/2;
		for(int j=0;j<h;++j) {
			for(int i=0;i<w;++i){
				SetLayout(i, j, minx+i*xdim, miny+j*ydim, 0, _mule.transform);
			}
		}

		SetLayout(0, 0, -2.0f, miny*-1.1f, -1, _mule.transform);
		SetLayout(0, 0, 2.0f, miny*-1.1f, 1, _mule.transform);

		_areaPos = new Vector3(0.0f,0.0f,miny*-1.1f);
		//_area.transform.position = new Vector3(0.0f,miny*1.5f,0.0f);
	}

	[EasyButtons.Button()]
	public void Kill() {
		DestroyImmediate(_mule);
	}


	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Limit() {
		if (_id < -1)
			_id = -1;
		if (_id >= _props.Count)
			_id = _props.Count-1;
	}
	void Area() {
		if(_id == -1) {
			if (_area != null)
				Destroy(_area);
			_area = null;
		}else {
			if (_area != null)
				Destroy(_area);
			_area = Instantiate(_props[_id],this.transform);
			_area.transform.position = _areaPos;
		}
	}

	public void Set(int x, int y) {
		if(_id == -1) {
			for(int i= _container._map[x, y].transform.childCount-1;i>=0;--i) {
				Destroy(_container._map[x, y].transform.GetChild(i).gameObject);
			}
			_container._map[x, y].GetComponent<TileCell>()._component.Clear();
		}else {
			GameObject prop = Instantiate(_props[_id],_container._map[x, y].transform);
			_container._map[x, y].GetComponent<TileCell>()._component.Add(prop);
		}
	}
	public void Left() {
		--_id;
		Limit();
		Area();
	}
	public void Right() {
		++_id;
		Limit();
		Area();
	}
}
