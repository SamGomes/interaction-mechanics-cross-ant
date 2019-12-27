using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReqScript : MonoBehaviour {
	public void UpdateRequirement(string word){
		gameObject.GetComponent<Text>().text = word;
	}
}
