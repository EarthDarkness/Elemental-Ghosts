using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupElement : ASignal<ElementBending> { }
public class ElementBending : MonoBehaviour
{

    public GameObject projectile;
    public float castingTime;
    private Coroutine castRoutine;

    public float timeDeath = 0.2f;

    public ElementTable.ElementType elementType
    {
        set
        {
            currentType = value;
            SendMessage("ChangePlayerElement", SendMessageOptions.DontRequireReceiver);
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

    public bool buffed = false;

    // Use this for initialization
    void Start()
    {
        currentType = ElementTable.ElementType.Neutral;
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
        currentType = ElementTable.ElementType.Neutral;
        Physics.IgnoreCollision(GetComponent<Collider>(), newProjectile.GetComponent<Collider>());
        SendMessage("ChangePlayerElement", SendMessageOptions.DontRequireReceiver);
    }

    public void Die()
    {
        // Animate
        // Particles
        // 
    }

    public IEnumerator _WaitToDie()
    {
        yield return new WaitForSeconds(timeDeath);
        gameObject.SetActive(false);
    }
}
