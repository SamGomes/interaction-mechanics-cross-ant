using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour {

    public float speed;
    public char letterText;
    private float totalTranslation;

    public bool isTranslationEnabled;

	void Awake() {
        totalTranslation = 0;
        isTranslationEnabled = true;
        speed = 1.5f;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isTranslationEnabled)
        {
            float translation = Time.deltaTime * speed;
            transform.Translate(translation, 0, 0);
            totalTranslation += translation;
        }
    }

    void DestroyLetter() {
        Destroy(this.gameObject);
    }
}
