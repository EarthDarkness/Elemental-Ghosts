using System.Collections.Generic;
using UnityEngine;


public class HitWall : ASignal<Projectile> { }
public class Projectile : MonoBehaviour
{

    private Rigidbody rb;
	bool hit = false;

    public EType type;
    public float force;
    public Vector3 direction;
    bool buffed = false;
    public List<ElementModel> listElement;
    private PlayerData parent;
    public void Initialize(PlayerData parent, Vector3 direction, EType elementType,
        bool buffed = false, float force = -1)
    {
        this.parent = parent;
        if (force != -1)
            this.force = force;
        type = elementType;
        this.direction = direction;
        this.buffed = buffed;

        ElementModel.ChangeModel(listElement, elementType);
    }

    // Use this for initialization
    void Start()
    {
        AudioManager.Instance.PlaySound(type.ToString());
        rb = GetComponent<Rigidbody>();

        rb.AddForce(direction * force, ForceMode.Impulse);

    }

    private void OnTriggerEnter(Collider other)
    {
		if (hit)
			return;
		
        if (other.CompareTag("Player"))
        {
            ElementBending otherPlayerElement = other.GetComponent<ElementBending>();
            //Debug.Log(type + " x " + otherPlayerElement.ElementType);
            ResolveInteraction(otherPlayerElement, Conflic.GetProjectileResult(type, otherPlayerElement.ElementType, buffed, otherPlayerElement.PlayerIsBuffed));
            Spawner._count -= 1;

            Destroy(this.gameObject);
        }
        else
        {
            Signals.Get<HitWall>().Dispatch(this);
            hit = true;
        }
    }

    void ResolveInteraction(ElementBending other, EResult result)
    {
		if(result == EResult.B_Damage) {
            other.GetComponent<PlayerData>().Die(parent);            
            other.ResetStates();
        }
        else if(result == EResult.B_Buff) {
            //Debug.Log("buff");
			other.PlayerIsBuffed = true;
            other.ChangeAura();
		}else {
            AudioManager.Instance.PlaySound("Miss");
            other.ResetStates();
            
		}

    }

}