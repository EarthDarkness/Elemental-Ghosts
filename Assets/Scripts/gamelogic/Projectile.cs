using UnityEngine;


public class HitWall : ASignal<Projectile> { }
public class Projectile : MonoBehaviour
{

    private Rigidbody rb;

    public ElementTable.ElementType type;
    public float force;
    public Vector3 direction;
    bool buffed = false;

    ElementBending otherPlayerElement;

    public void Initialize(Vector3 direction, ElementTable.ElementType elementType, bool buffed = false, float force = -1)
    {
        if (force != -1)
            this.force = force;
        type = elementType;
        this.direction = direction;
        this.buffed = buffed;
    }

    // Use this for initialization
    void Start()
    {

        rb = GetComponent<Rigidbody>();

        rb.AddForce(direction * force, ForceMode.Impulse);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            otherPlayerElement = other.GetComponent<ElementBending>();
            Debug.Log(type + " x " + otherPlayerElement.elementType);
            ResolveInteraction(ElementTable.GetProjectileResult(type, otherPlayerElement.elementType, buffed, otherPlayerElement.playerIsBuffed));
            Spawner._count -= 1;
            Destroy(this.gameObject);
        }
        else
        {
            Signals.Get<HitWall>().Dispatch(this);
        }
    }

    void ResolveInteraction(ElementTable.FightState result)
    {
        Debug.Log(result);
        switch (result)
        {
            case ElementTable.FightState.Annulment:
                otherPlayerElement.elementType = ElementTable.ElementType.Neutral;
                break;
            case ElementTable.FightState.Buffed:
                otherPlayerElement.playerIsBuffed = true;
                break;
            case ElementTable.FightState.Destroy:
                otherPlayerElement.gameObject.SetActive(false);
                break;
            case ElementTable.FightState.Nothing:
                break;
        }
    }

}
