﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ElementModel
{

    public GameObject gameObject;
    public ElementTable.ElementType elementType;

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);

    }

    public static void ChangeModel(List<ElementModel> list, ElementTable.ElementType elementType)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetActive(list[i].elementType == elementType);
        }
    }
}
