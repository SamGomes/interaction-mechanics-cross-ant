using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillLeters : MonoBehaviour {

    void OnTriggerEnter(Collider otherObject)
    {
        Destroy(otherObject.GetComponent<Image>().gameObject);
        Destroy(otherObject.gameObject);
    }
}
