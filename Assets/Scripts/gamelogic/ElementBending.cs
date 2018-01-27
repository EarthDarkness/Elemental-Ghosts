using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupElement : ASignal<ElementBending> { }
public class ElementBending : MonoBehaviour
{

    public GameObject projectile;
    public float castingTime;
    [InspectorReadOnly]
    private Coroutine castRoutine;

    [InspectorReadOnly]
    private GameObject[] playerModels = new GameObject[6];

    
    public ElementTable.ElementType elementType
    {
        set
        {
            currentType = value;
            ChangePlayerElement();
            //SendMessage("ChangePlayerElement", SendMessageOptions.DontRequireReceiver);
        }
        get { return currentType; }
    }

   
    public ElementTable.ElementType currentType = ElementTable.ElementType.Neutral;

    public bool playerIsBuffed
    {
        set
        {
            buffed = value;
        }
        get { return buffed; }
    }

    public bool buffed = false;

    // Use this for initialization
    void Start()
    {
        currentType = ElementTable.ElementType.Neutral;

        for (int i = 0; i < gameObject.transform.GetChild(0).childCount; i++)
        {
            playerModels[i] = gameObject.transform.GetChild(0).GetChild(i).gameObject;
        }
        ChangePlayerElement();
    }


    private void Action()
    {
        if (currentType == ElementTable.ElementType.Neutral)
        {
            Signals.Get<PickupElement>().Dispatch(this);
        }
        else
        {
            castRoutine = StartCoroutine(Shoot());
        }
    }

    public IEnumerator Shoot()
    {
        yield return new WaitForSeconds(castingTime);
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.LookRotation(transform.forward, transform.up));
        newProjectile.GetComponent<Projectile>().Initialize(transform.forward, currentType, this.buffed);
        currentType = ElementTable.ElementType.Neutral;
        Physics.IgnoreCollision(GetComponent<Collider>(), newProjectile.GetComponent<Collider>());
        ChangePlayerElement();
        //SendMessage("ChangePlayerElement", SendMessageOptions.DontRequireReceiver);
    }

    void ChangePlayerElement()
    {
        DeactivateModels();
        playerModels[(int)elementType].SetActive(true);

    }

    void DeactivateModels()
    {
        for (int i = 0; i < playerModels.Length; i++)
        {
            playerModels[i].SetActive(false);
        }
    }

}
