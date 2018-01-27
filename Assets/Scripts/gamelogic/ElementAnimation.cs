using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementAnimation : MonoBehaviour
{

    public GameObject prefab;

    GameObject instantiatedPrefab;

    public void InstantiateAnimation()
    {
        instantiatedPrefab = Instantiate(prefab, transform);
        Destroy(gameObject, 1.5f);
    }

}
