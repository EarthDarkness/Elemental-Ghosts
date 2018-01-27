using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickupElement : ASignal<ElementBending> { }
public class ElementBending : MonoBehaviour
{
    public GameObject projectile;
    public float castingTime;
    private Coroutine castRoutine;
    public bool buffed = false;
    public List<ElementModel> listElement;


    public ElementTable.ElementType elementType
    {
        set
        {
            currentType = value;
            ChangeModel();

        }
        get { return currentType; }
    }
    private ElementTable.ElementType currentType = ElementTable.ElementType.Neutral;

    public bool playerIsBuffed
    {
        set
        {
            buffed = value;
        }
        get { return buffed; }
    }


    // Use this for initialization
    void Start()
    {
        elementType = ElementTable.ElementType.Neutral;

    }


    public void Action()
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
        elementType = ElementTable.ElementType.Neutral;
        Physics.IgnoreCollision(GetComponent<Collider>(), newProjectile.GetComponent<Collider>());
    }

    public void ChangeModel()
    {
        ElementModel.ChangeModel(listElement, currentType);
    }

}
