using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Player
{
    [SerializeField]
    private List<float> buttonRGB;
    private Color buttonColor;

    [SerializeField]
    private string name;

    [SerializeField]
    private List<KeyCode> myKeys;

    [SerializeField]
    private List<string> myButtons;
   
    private int score;
    private int activeButtonIndex;

    private Globals.KeyInteractionType activeInteraction;

    private GameObject wordPanel;
    private GameObject scorePanel;

    private GameObject marker;
    private List<GameObject> maskedHalf;

    private GameManager gameManagerRef;

    private IEnumerator currButtonLerp;


    private Exercise currExercise;
    public bool isCurrExerciseFinished;
    private string currWordState;

    public Player(List<KeyCode> myKeys, List<string> myButtons, List<float> buttonRGB)
    {
        this.buttonRGB = buttonRGB;

        this.myKeys = myKeys;
        this.myButtons = myButtons;
    }

    public void Init(GameManager gameManagerRef, GameObject markerPrefab, GameObject canvas, GameObject wordPanel, GameObject scorePanel, bool isTopMask)
    {
        //this.buttonColor = new Color(buttonRGB[0], buttonRGB[1], buttonRGB[2], 1.0f);
        this.buttonColor = new Color(buttonRGB[0], buttonRGB[1], buttonRGB[2], 0.8f);
        //marker.transform.position = activeButtonPos;
        this.gameManagerRef = gameManagerRef;

        marker = UnityEngine.Object.Instantiate(markerPrefab, canvas.transform);
        //marker.transform.Rotate(new Vector3(1, 0, 0), angle);
        foreach (Image image in marker.GetComponentsInChildren<Image>()) {
            image.color = this.buttonColor;
        }

        maskedHalf = new List<GameObject>();
        maskedHalf.Add((isTopMask == true) ? marker.transform.Find("Button/BackgroundTH").gameObject : marker.transform.Find("Button/BackgroundBH").gameObject);
        maskedHalf.Add((isTopMask == true) ? marker.transform.Find("trackTH").gameObject : marker.transform.Find("trackBH").gameObject);

        this.wordPanel = wordPanel;
        this.scorePanel = scorePanel;


        scorePanel.GetComponentInChildren<Image>().color = this.buttonColor;

        this.name = "";
        this.score = -1;
    }
    
    public void SetCurrExercise(Exercise newExercise)
    {
        //currWordState = "";
        currExercise = newExercise;
    }
    public Exercise GetCurrExercise()
    {
        return currExercise;
    }

    public void SetCurrWordState(string newCurrWordState)
    {
        currWordState = newCurrWordState;

        //update UI
        string displayString = "";
        int missingLength = currExercise.targetWord.Length - currWordState.Length;
        string[] substrings = currExercise.displayMessage.Split('_');

        if (substrings.Length > 0)
        {
            displayString += substrings[0];
            displayString += currWordState;
            for (int i = 0; i < missingLength; i++)
            {
                displayString += "_";
            }
            if (substrings.Length > 1)
            {
                displayString += substrings[1];
            }
        }
        displayString += "\n";

        Text playerDisplayText = wordPanel.GetComponentInChildren<Text>();
        playerDisplayText.text = displayString;
    }
    public string GetCurrWordState()
    {
        return currWordState;
    }

    public void SetScore(int score)
    {
        if(this.score != score)
        {
            this.score = score;

            //update UI
            Text playerPanelText = scorePanel.GetComponentInChildren<Text>();
            playerPanelText.text = "Player " + name + " Score: " + score;
            scorePanel.GetComponent<Animator>().Play(0);
        }

    }
    public int GetScore()
    {
        return this.score;
    }

    public void SetScorePanel(GameObject scorePanel)
    {
        this.scorePanel = scorePanel;
    }
    public void SetWordPanel(GameObject wordPanel)
    {
        this.wordPanel = wordPanel;
    }
    public GameObject GetScorePanel()
    {
        return this.scorePanel;
    }
    public GameObject GetWordPanel()
    {
        return this.wordPanel;
    }

    public string GetName()
    {
        return this.name;
    }
    public List<KeyCode> GetMyKeys() {
        return myKeys;
    }
    public List<string> GetMyButtons()
    {
        return myButtons;
    }

    public void SetName(string name)
    {
        this.name = name;

        //update UI
        Text playerPanelText = scorePanel.GetComponentInChildren<Text>();
        playerPanelText.text = "Player " + name + " Score: " + score;
    }

    public int GetActivebuttonIndex()
    {
        return activeButtonIndex;
    }

    public void SetActiveButton(int activeButtonIndex, Vector3 activeButtonPos)
    {
        if (currButtonLerp != null)
        {
            gameManagerRef.StopCoroutine(currButtonLerp);
        }

        currButtonLerp = Globals.LerpAnimation(marker, activeButtonPos, 3.5f);
        gameManagerRef.StartCoroutine(currButtonLerp);

        this.activeButtonIndex = activeButtonIndex;
    }

    public List<GameObject> GetMaskedHalf()
    {
        return this.maskedHalf;
    }

    public void SetActiveInteraction(Globals.KeyInteractionType activeInteraction)
    {
        this.activeInteraction = activeInteraction;
    }
    public Globals.KeyInteractionType GetActiveInteraction()
    {
        return activeInteraction;
    }


    public Color GetButtonColor()
    {
        return buttonColor;
    }

    public GameObject GetMarker()
    {
        return this.marker;
    }

}