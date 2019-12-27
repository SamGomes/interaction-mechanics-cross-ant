using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour {

    public float speed;
    public char letterText;
    private float totalTranslation = 0;

	void Awake() {
        this.speed = 1.5f;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float translation = Time.deltaTime * speed;
        transform.Translate(translation, 0, 0);
        totalTranslation += translation;
        
    }

    void DestroyLetter() {
        Destroy(GetComponent<SpriteRenderer>().gameObject);
        Destroy(this.gameObject);
    }
}
