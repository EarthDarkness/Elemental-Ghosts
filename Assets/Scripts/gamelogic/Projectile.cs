using UnityEngine;

public class Projectile : MonoBehaviour {

    private Rigidbody rb;

    public ElementTable.ElementType type;
    public float force;
    public Vector3 direction;

    public void Initialize(Vector3 direction, ElementTable.ElementType elementType, float force = -1)
    {
        if(force != -1)
            this.force = force;
        type = elementType;
        this.direction = direction;
    }

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(direction * force, ForceMode.Impulse);

	}
	
}
