using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileCell : MonoBehaviour{
	public bool _lock = false;
	public List<GameObject> _component = new List<GameObject>();
	public GameObject _elementVisual = null;
	public int _elementType = 5;//5 = no element (TODO element type enum)


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

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }



};