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

    [InspectorReadOnly]
    private GameObject[] playerModels = new GameObject[6];

	public static float pickupbasetimer = 0.5f;  
	public float elementalPickup = 0.0f;  


    public float timeDeath = 0.2f;

    private PlayerData playerData;

    public ElementTable.ElementType ElementType
    {
        set
        {
            currentType = value;
            ChangeModel();
        }
        get { return currentType; }
    }


    public ElementTable.ElementType currentType = ElementTable.ElementType.Neutral;

    public bool PlayerIsBuffed
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
        playerData = GetComponent<PlayerData>();
        ElementType = ElementTable.ElementType.Neutral;


        for (int i = 0; i < gameObject.transform.GetChild(0).childCount; i++)
        {
            playerModels[i] = gameObject.transform.GetChild(0).GetChild(i).gameObject;
        }

    }
	void Update() {  
		if (elementalPickup > 0.0f)  
			elementalPickup -= Time.deltaTime;  
	}  

    public void Action()
    {
        if (currentType == ElementTable.ElementType.Neutral)
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
        yield return new WaitForSeconds(castingTime);
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.LookRotation(transform.forward, transform.up));
        newProjectile.GetComponent<Projectile>().Initialize(playerData, transform.forward, currentType, this.buffed);
        ElementType = ElementTable.ElementType.Neutral;
        Physics.IgnoreCollision(GetComponent<Collider>(), newProjectile.GetComponent<Collider>());

    }

    public void ChangeModel()
    {
        ElementModel.ChangeModel(listElement, currentType);

    }


}
