using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Button : MonoBehaviour {

    public GameManager gameManager;
    public Utilities.ButtonId buttonCode;

    private bool keyPressed;
    private bool clicked;
    private bool locked;

    private List<Utilities.PlayerId> playersPressingThisButton;

    public void RegisterUserButtonPress(Utilities.PlayerId[] playersPressingThisButton) {
        this.playersPressingThisButton = new List<Utilities.PlayerId>(playersPressingThisButton);
        List<Player> players = gameManager.GetPlayers();
        this.keyPressed = true;
    }

    private void Start()
    {
        this.locked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.keyPressed && !this.locked)
        {
            this.clicked = true;
            this.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        else
        {
            this.playersPressingThisButton = new List<Utilities.PlayerId>();
            this.clicked = false;
            this.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }

        if (!this.keyPressed)
        {
            this.locked = false;
        }

        this.keyPressed = false;

    }


    void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.GetComponent<Letter>()== null){
            return;
        }
        if (this.clicked)
        {
            otherObject.gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 1.0f);
            otherObject.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;

            gameManager.RecordHit(this.playersPressingThisButton, otherObject.gameObject.GetComponent<Letter>().letterText);

            gameObject.GetComponent<AudioSource>().Play();
        }
        this.locked = true;
    }
}
