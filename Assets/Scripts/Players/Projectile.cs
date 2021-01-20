using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float moveSpeed = 10f;
    public Rigidbody myRB;
    public float deathTime = 3f;

	// Use this for initialization
	void Start ()
    {
        myRB.velocity = new Vector3(moveSpeed, myRB.velocity.y, myRB.velocity.z);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (deathTime < Time.deltaTime)
            Destroy(gameObject);
	}
}
