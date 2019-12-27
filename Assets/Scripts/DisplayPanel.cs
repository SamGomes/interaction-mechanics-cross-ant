using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPanel : MonoBehaviour {
    public void SetTargetImage(string currWord)
    {
        GameObject.Find("food").GetComponent<SpriteRenderer>().sprite = (Sprite) Resources.Load("Textures/FoodItems/" + currWord, typeof(Sprite));
    }
}
