using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIMainMenu : MonoBehaviour
{

    public float timer = 3.0f;
    public ElementBending elementModel;
    public EType currentType;
   

    public void Start()
    {
        elementModel = GetComponent<ElementBending>();
        ChangeType();
        StartCoroutine(_timer());
    }

    public IEnumerator _timer()
    {
        yield return new WaitForSeconds(timer);
        ChangeType();
        StartCoroutine(_timer());
    }

    [EasyButtons.Button()]
    public void ChangeType()
    {
        currentType = (EType)(((int)currentType + Random.Range(1, 6)) % 6);
        elementModel.currentType = currentType;
        elementModel.ChangeModel();
    }

}
