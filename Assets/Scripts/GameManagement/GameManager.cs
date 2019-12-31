using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities
{
    public enum PlayerId
    {
        PLAYER_0,
        PLAYER_1,
        NONE
    }

    public enum ButtonId
    {
        BTN_0,
        BTN_1,
        BTN_2,
        NONE
    }

    public enum PlayersToPressButtonAlternative
    {
        SINGLE_PLAYER,
        MULTIPLAYER
    }

    public enum OutputRestriction
    {
        EAT,
        STARPOWER,
        NONE
    }

    public static PlayersToPressButtonAlternative[] playersToPressButtonAlternatives = (PlayersToPressButtonAlternative[]) Enum.GetValues(typeof(Utilities.PlayersToPressButtonAlternative));

    public static ButtonId[] buttonIds = (ButtonId[]) Enum.GetValues(typeof(Utilities.ButtonId));

    public static OutputRestriction[] outputRestrictions = (OutputRestriction[])Enum.GetValues(typeof(Utilities.OutputRestriction));

    public static int numPlayersToPressButtonAlternatives = playersToPressButtonAlternatives.Length;
    public static int simultaneousKeysToPress = 2;

    public static int numOutputRestriction = outputRestrictions.Length;
}


public class GameManager : MonoBehaviour
{

    public GameSceneManager gameSceneManager;

    public bool isGameplayPaused;

    public Text hpPanel;
    public SpriteRenderer foodSprite;
    public Text scorePanel;
    public Text timePanel;
    public Text reqPanel;
//    public GameObject track;

    public InputManager inputManager;


    public GameObject playersPanel;
    public Text[] playersPanelTexts;
    public AntSpawner[] antSpawners;
    public LetterSpawner[] letterSpawners;
    public List<Button> gameButtons;

    public string currWord;

    public float timeLeft;

    public int lifes;
    private int score;
    private List<Exercise> exercises;

    private List<Player> players;
    private List<Utilities.PlayerId> lastPlayersToPressIndexes;

    private List<Utilities.OutputRestriction> prevAntOutputs;
    public Utilities.PlayersToPressButtonAlternative currNumPlayersCombo;

    public Exercise currExercise { get; set; }

    public void PauseGame()
    {
        Time.timeScale = 0;
        isGameplayPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isGameplayPaused = false;
    }


    public void PL1NameBoxTextChanged(string newText)
    {
        players[0].SetName(newText);
    }
    public void PL2NameBoxTextChanged(string newText)
    {
        players[1].SetName(newText);
    }
   

    // Use this for initialization
    void Start()
    {

        playersPanelTexts = playersPanel.transform.GetComponentsInChildren<UnityEngine.UI.Text>();
        
        lastPlayersToPressIndexes = new List<Utilities.PlayerId>();

        isGameplayPaused = false;
        gameSceneManager.MainSceneLoadedNotification();

        players = new List<Player>();
        players.Add(new Player(Utilities.PlayerId.PLAYER_0, new KeyCode[] { KeyCode.Q, KeyCode.W, KeyCode.E }, new string[] { "YButtonJoy1" , "BButtonJoy1" }));
        players.Add(new Player(Utilities.PlayerId.PLAYER_1, new KeyCode[] { KeyCode.I, KeyCode.O, KeyCode.P }, new string[] { "YButtonJoy2" , "BButtonJoy2" }));


        exercises = new List<Exercise>();
        exercises.Add(new Exercise("Word to match: CAKE \n Your Word:_", "CAKE"));
        exercises.Add(new Exercise("Word to match: BANANA \n Your Word:_", "BANANA"));
        exercises.Add(new Exercise("Word to match: PIE \n Your Word:_", "PIE"));
        exercises.Add(new Exercise("Word to match: PIZZA \n Your Word:_", "PIZZA"));
        exercises.Add(new Exercise("Word to match: CROISSANT \n Your Word:_", "CROISSANT"));
        exercises.Add(new Exercise("Word to match: DONUT \n Your Word:_", "DONUT"));
        exercises.Add(new Exercise("Word to match: CHERRY \n Your Word:_", "CHERRY"));
        exercises.Add(new Exercise("Word to match: XMASCOOKIES \n Your Word:_", "XMASCOOKIES"));
        exercises.Add(new Exercise("Word to match: KIWI \n Your Word:_", "KIWI"));
        exercises.Add(new Exercise("Word to match: QUICHE \n Your Word:_", "QUICHE"));
        exercises.Add(new Exercise("Word to match: MANGO \n Your Word:_", "MANGO"));
        exercises.Add(new Exercise("Word to match: FISH \n Your Word:_", "FISH"));
        exercises.Add(new Exercise("Word to match: VANILLA \n Your Word:_", "VANILLA"));
        exercises.Add(new Exercise("Word to match: JELLY \n Your Word:_", "JELLY"));

        lifes = 50;

        timeLeft = 100.0f;
        InvokeRepeating("DecrementTimeLeft", 0.0f, 1.0f);

        ChangeTargetWord();
        ChangeGameParametrizations(true);

        prevAntOutputs = new List<Utilities.OutputRestriction>(2);
        prevAntOutputs.Add(Utilities.OutputRestriction.NONE);
        prevAntOutputs.Add(Utilities.OutputRestriction.NONE);

        gameSceneManager.StartAndPauseGame(Utilities.PlayerId.NONE); //for the initial screen
    
    }


    // Update is called once per frame
    void Update()
    {
        if (isGameplayPaused)
        {
            return;
        }

        //if no lifes end game immediately
        if (lifes < 1)
        {
            gameSceneManager.EndGame();
        }

        //if time's up change word
        if (timeLeft == 0.0f)
        {
            ChangeLevel();
            timeLeft = 100.0f;
            Hurt();
        }

        hpPanel.text = "Lifes: "+ lifes;
        scorePanel.text = "Team Score: "+ score;
        timePanel.text = "Time: "+ timeLeft;

        for(int i=0; i < players.Count; i++)
        {
            playersPanelTexts[i].text = "Player "+ 
                players[i].GetName()+" Score: " + players[i].score;
        }

        //update curr display message
        int missingLength = this.currExercise.targetWord.Length - currWord.Length;
        string[] substrings = this.currExercise.displayMessage.Split('_');

        string displayString = "";
        if (substrings.Length > 0)
        {
            displayString = substrings[0];
            displayString += currWord;
            for (int i = 0; i < missingLength; i++)
            {
                displayString += "_";
            }
            if (substrings.Length > 1)
            {
                displayString += substrings[1];
            }
        }
        reqPanel.text = displayString;
    }



    void ChangeLevel()
    {
        PoppupQuestionnaires();
        ChangeTargetWord();
        ChangeGameParametrizations(false);
    }

    void DecrementTimeLeft()
    {
        if(timeLeft > 0.0f){
            timeLeft--; 
        }
    }

    void PoppupQuestionnaires()
    {
        gameSceneManager.pauseForQuestionnaires(Utilities.PlayerId.NONE);
        //spawn questionnaires before changing word
        foreach (Player player in players)
        {
            //Debug.Log("window.open('https://docs.google.com/forms/d/e/1FAIpQLSeM3Xn5qDBdX7QCtyrPILLbqpYj3ueDcLa_-9CbxCPzxVsMzg/viewform?usp=pp_url&entry.100873100=" + player.GetName() + "&entry.2097900814=" + player.GetId() + "&entry.631185473=" + currExercise.targetWord + "&entry.159491668=" + (int)this.currNumPlayersCombo + "&entry.1472728103=" + (int)prevAntOutputs[players.IndexOf(player)] + "');");
            Application.ExternalEval("window.open('https://docs.google.com/forms/d/e/1FAIpQLSeM3Xn5qDBdX7QCtyrPILLbqpYj3ueDcLa_-9CbxCPzxVsMzg/viewform?usp=pp_url&entry.100873100=" + player.GetName() + "&entry.2097900814=" + player.GetId() + "&entry.631185473=" + currExercise.targetWord + "&entry.159491668=" + (int)this.currNumPlayersCombo+ "&entry.1472728103=" + (int)prevAntOutputs[players.IndexOf(player)] + "');"); //spawn questionaires
        }
    }

    void ChangeGameParametrizations(bool firstTimeCall)
    {
        inputManager.InitKeys();
        
        int numKeysToPress = Utilities.simultaneousKeysToPress;
        this.currNumPlayersCombo = firstTimeCall ? Utilities.PlayersToPressButtonAlternative.SINGLE_PLAYER : ChooseNumPlayersCombo();
        foreach (Player player in this.players)
        {
            List<KeyCode> possibleKeys = new List<KeyCode>();
            if (this.currNumPlayersCombo == Utilities.PlayersToPressButtonAlternative.SINGLE_PLAYER)
            {
                possibleKeys = new List<KeyCode>(player.GetMyKeys());
            }
            else if (this.currNumPlayersCombo == Utilities.PlayersToPressButtonAlternative.MULTIPLAYER)
            {
                possibleKeys = new List<KeyCode>();
                foreach (Player innerPlayer in this.players)
                {
                    possibleKeys.AddRange(innerPlayer.GetMyKeys());
                }
            }

            List<HashSet<KeyCode>> buttonKeyCombos = new List<HashSet<KeyCode>>();
            foreach (Button button in this.gameButtons)
            {
                HashSet<KeyCode> buttonKeyCombo = new HashSet<KeyCode>();
                while (buttonKeyCombo.Count < numKeysToPress)
                {
                    int randomIndex = UnityEngine.Random.Range(0, possibleKeys.Count);
                    KeyCode currCode = possibleKeys[randomIndex];
                    //possibleKeys.RemoveAt(randomIndex);
                    buttonKeyCombo.Add(currCode);

                    //ensure that no two equal key combinations are generated
                    foreach (HashSet<KeyCode> hash in buttonKeyCombos)
                    {
                        if (hash.SetEquals(buttonKeyCombo))
                        {
                            buttonKeyCombo = new HashSet<KeyCode>();
                        }
                    }
                }
                buttonKeyCombos.Add(buttonKeyCombo);
                inputManager.AddKeyBinding(new List<KeyCode>(buttonKeyCombo).ToArray(), InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)button.buttonCode].RegisterUserButtonPress(new Utilities.PlayerId[] { player.GetId() }); });
            }
        }

        for(int i=0; i<letterSpawners.Length; i++)
        {
            if(!firstTimeCall && letterSpawners[i].minIntervalRange > 0.3 && letterSpawners[i].maxIntervalRange > 0.4)
            {
                letterSpawners[i].minIntervalRange -= 0.1f;
                letterSpawners[i].maxIntervalRange -= 0.1f;
            }
        }


//        //change ants modes
//        prevAntOutputs = new List<Utilities.OutputRestriction>();
//        track.GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("Textures/track", typeof(Sprite));
//
//        string targetWord = this.currExercise.targetWord;
//
//        for (int i = 0; i < letterSpawners.Length; i++)
//        {
//            letterSpawners[i].UpdateCurrStarredWord("");
//
//        }
//        for (int i = 0; i < antSpawners.Length; i++)
//        {
//            Utilities.OutputRestriction currOutputRestriction = antSpawners[i].GetComponent<AntSpawner>().outputRestriction;
//            prevAntOutputs.Add(currOutputRestriction);
//
//            if (currOutputRestriction == Utilities.OutputRestriction.STARPOWER)
//            {
//                //change the track on star power
//                track.GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("Textures/starTrack", typeof(Sprite));
//                letterSpawners[UnityEngine.Random.Range(0,letterSpawners.Length)].UpdateCurrStarredWord(targetWord);
//            }
//
//            antSpawners[i].GetComponent<AntSpawner>().outputRestriction = ChooseOutputRestriction(); //Utilities.OutputRestriction.STARPOWER;
//        }

    }

    void ChangeTargetWord()
    {
        int random = UnityEngine.Random.Range(0, exercises.Count);
        Exercise newExercise = exercises[random];

        currWord = "";
        
        foodSprite.sprite = (Sprite) Resources.Load("Textures/FoodItems/" + newExercise.targetWord, typeof(Sprite));
        this.currExercise = newExercise;
    }

    void Hurt()
    {
        lifes--;
    }

    public void RecordHit(List<Utilities.PlayerId> hitters, char letterText)
    {
        this.lastPlayersToPressIndexes = hitters;

        this.currWord += letterText;
        this.currWord = this.currWord.ToUpper();
        string currTargetWord = this.currExercise.targetWord;


        if (currWord.Length <= currTargetWord.Length && currTargetWord[currWord.Length - 1] == currWord[currWord.Length - 1])
        {
            score += 100;
            foreach (Utilities.PlayerId playerId in lastPlayersToPressIndexes)
            {
                players[(int)playerId].score += 50;
            }
        }
        else
        {
            Hurt();
            currWord = currWord.Remove(currWord.Length - 1);
            return;
        }
        

        if (currWord.CompareTo(currTargetWord) == 0)
        {
            timeLeft += currTargetWord.Length*4;
            timeLeft = 100.0f;

            //init track and play ant anims
            GameObject[] letters = GameObject.FindGameObjectsWithTag("letter");
            foreach (GameObject letter in letters)
            {
                Destroy(letter);
            }
            foreach (LetterSpawner letterSpawner in letterSpawners)
            {
                letterSpawner.SetScore(score);
            }
            foreach (AntSpawner antSpawner in antSpawners)
            {
                antSpawner.SpawnAnt(currTargetWord);
            }

            ChangeLevel();

        }
    }


    private Utilities.PlayersToPressButtonAlternative ChooseNumPlayersCombo()
    {
        int randomIndex = UnityEngine.Random.Range(0, Utilities.numPlayersToPressButtonAlternatives);
        return Utilities.playersToPressButtonAlternatives[randomIndex];

    }
    private Utilities.OutputRestriction ChooseOutputRestriction()
    {
        int randomIndex = UnityEngine.Random.Range(0, Utilities.numOutputRestriction);
        return Utilities.outputRestrictions[randomIndex];
    }


    public List<Player> GetPlayers()
    {
        return this.players;
    }


}


