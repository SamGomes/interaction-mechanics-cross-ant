using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillLeters : MonoBehaviour {

    void OnTriggerEnter(Collider otherObject)
    {
        Destroy(otherObject.GetComponent<SpriteRenderer>().gameObject);
        Destroy(otherObject.gameObject);
    }
}
