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

    public ElementTable.ElementType elementType
    {
        set
        {
            currentType = value;
            SendMessage("ChangePlayerElement");
        }
        get { return currentType; }
    }
    private ElementTable.ElementType currentType = ElementTable.ElementType.Neutral;

    // Use this for initialization
    void Start()
    {
        currentType = ElementTable.ElementType.Neutral;
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
        GameObject newProjectile = Instantiate(projectile);
        newProjectile.GetComponent<Projectile>().Initialize(transform.forward, currentType);
        currentType = ElementTable.ElementType.Neutral;
        SendMessage("ChangePlayerElement");
    }
}
