using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileCell : MonoBehaviour
{
    public bool _lock = false;
    public List<GameObject> _component = new List<GameObject>();
    public GameObject _elementVisual = null;
    public EType _elementType = EType.Neutral;


    /*void Awake() {
		for(int i=0;i<transform.childCount;++i) {
			_component.Add(transform.GetChild(i).gameObject);
		}
	}*/

    void OnValidate()
    {

        if (_lock)
            return;
        for (int i = transform.childCount; i < _component.Count; ++i)
        {
            //if(_component[i] == null) {
            //	_component.RemoveAt(i);
            //}else {
            if (_component[i] != null)
                Instantiate(_component[i], transform);
            //}
        }
        /*for(int i=0;i<_component.Count;++i) {
			if (transform.childCount > i || _component[i] == null)
				continue;
			_
		}*/
    }

    public TileCell()
    {

    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }

    public void Reset()
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = transform.GetChild(i);
        }

        for (int i = 0; i < children.Length; i++)
        {
            DestroyImmediate(children[i].gameObject);
        }

        for (int i = 0; i < _component.Count; ++i)
        {
            Debug.Log(_component[i]);
            if (_component[i] != null)
                Instantiate(_component[i], transform);
        }


    }



};