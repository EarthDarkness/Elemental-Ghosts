using UnityEngine;


public class HitWall : ASignal<Projectile> { }
public class Projectile : MonoBehaviour
{
    
    private Rigidbody rb;

    public ElementTable.ElementType type;
    public float force;
    public Vector3 direction;
    bool buffed = false;

    Board board;

    public void Initialize(Vector3 direction, ElementTable.ElementType elementType, bool buffed = false ,float force = -1)
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
            ElementTable.ElementType otherElement = other.GetComponent<ElementBending>().elementType;
            bool otherBuff = other.GetComponent<ElementBending>().playerIsBuffed;
            ResolveInteraction(ElementTable.GetProjectileResult(type, otherElement,buffed,otherBuff));
            Destroy(this.gameObject);
        }
        else
        {
            Signals.Get<HitWall>().Dispatch(this);
        }
    }

    void ResolveInteraction(ElementTable.FightState result)
    {
        switch (result)
        {
            case ElementTable.FightState.Annulment:
                break;
            case ElementTable.FightState.Buffed:
                break;
            case ElementTable.FightState.Destroy:
                break;
            case ElementTable.FightState.Nothing:
                break;
        }
    }

}
