using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private Utilities.PlayerId id;
    private string name;

    private KeyCode[] myKeys;
    private string[] myButtons;

    public int score;

    public Player(Utilities.PlayerId id, KeyCode[] myKeys, string[] myButtons)
    {
        this.myKeys = myKeys;
        this.myButtons = myButtons;

        this.id = id;
        this.name = "";
        this.score = 0;
    }

    public Utilities.PlayerId GetId()
    {
        return this.id;
    }
    public string GetName()
    {
        return this.name;
    }
    public KeyCode[] GetMyKeys() {
        return myKeys;
    }
    public string[] GetMyButtons()
    {
        return myButtons;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

}