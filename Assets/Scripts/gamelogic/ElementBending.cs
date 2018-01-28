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
    public List<ElementModel> auraModel;

	public static float pickupbasetimer = 0.5f;  
	public float elementalPickup = 0.0f;  


    public float timeDeath = 0.2f;

    private PlayerData playerData;
    public Animator animator;

    public EType ElementType
    {
        set
        {
            currentType = value;
            ChangeModel();
        }
        get { return currentType; }
    }


    public EType currentType = EType.Neutral;

    public bool PlayerIsBuffed
    {
        set
        {
            buffed = value;
        }
        get { return buffed; }
    }


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        playerData = GetComponent<PlayerData>();
        ElementType = EType.Neutral;
    }
	void Update() {
		if (elementalPickup > 0.0f)  
			elementalPickup -= Time.deltaTime;  
	}  

    public void Action()
    {
        if (currentType == EType.Neutral)
        {
            //Signals.Get<PickupElement>().Dispatch(this);
			elementalPickup = pickupbasetimer; 
        }
        else
        {
            castRoutine = StartCoroutine(Shoot());
        }
    }

    public IEnumerator Shoot()
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(castingTime);
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.LookRotation(transform.forward, transform.up));
        newProjectile.GetComponent<Projectile>().Initialize(playerData, transform.forward, currentType, this.buffed);
        ResetStates();
        Physics.IgnoreCollision(GetComponent<Collider>(), newProjectile.GetComponent<Collider>());

    }

    public void ResetStates()
    {
        ElementType = EType.Neutral;
        PlayerIsBuffed = false;
        DisableAura();
    }

    public void ChangeModel()
    {
        ElementModel.ChangeModel(listElement, currentType);
    }

    public void ChangeAura()
    {
        ElementModel.ChangeModel(auraModel, currentType);
    }

    public void DisableAura()
    {
        for (int i = 0; i < auraModel.Count; i++)
        {
            auraModel[i].SetActive(false);
        }
    }


}
