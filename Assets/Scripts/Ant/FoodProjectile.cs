using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodProjectile : MonoBehaviour {
    

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 0.4f);
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(0.5f, 1.5f, 0.0f));
	}

   
}
