using UnityEngine;

public class Projectile : MonoBehaviour {

    Rigidbody rb;

    ElementTable.ElementType type;

    

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        rb.AddForce(player.transform.forward * 10, ForceMode.Impulse);

	}
	
	// Update is called once per frame
	void Update () {
     
	}
}
