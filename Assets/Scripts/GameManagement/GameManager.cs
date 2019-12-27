using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class Globals
{
    public enum PlayerId
    {
        PLAYER_0,
        PLAYER_1,
        PLAYER_2,
        NONE
    }

    public enum ButtonId
    {
        BTN_0,
        BTN_1,
        BTN_2,
        NONE
    }
    
    public enum KeyInteractionType
    {
        NONE,
        TAKE,
        GIVE
    }

    public static KeyInteractionType[] keyInteractionTypes = (KeyInteractionType[]) Enum.GetValues(typeof(Globals.KeyInteractionType));
    public static ButtonId[] buttonIds = (ButtonId[]) Enum.GetValues(typeof(Globals.ButtonId));
    
    public static int currLevelId = 0;


    public static IEnumerator LerpAnimation(GameObject source, Vector3 targetPos, float speed)
    {
        Vector3 sourcePos = source.transform.position;
        float totalDist = (targetPos - sourcePos).sqrMagnitude;
        float currT = 0;
        while (currT < 1.0f && source != null)
        {
            source.transform.position = Vector3.Lerp(sourcePos, targetPos, currT += speed * 0.025f);
            yield return new WaitForSeconds(0.025f);
        }
    }
}

[Serializable]
public class ScoreValue
{
    public bool usefulForMe;
    public bool usefulForOther;
    public int myValue;
    public int otherValue;
}
[Serializable]
public class ScoreSystem
{
    public List<ScoreValue> giveScores;
    public List<ScoreValue> takeScores;

    public int completeWordMyScore;
    public int completeWordOtherScore;
}

[Serializable]
public class ExercisesListWrapper
{
    public List<Exercise> exercises;
}
[Serializable]
public struct GameSettings
{
    public List<ExercisesListWrapper> exercisesGroups;
    public List<Player> players;
    public int gameId;
    public int initialTimeLeft;

    public ScoreSystem scoreSystem;
}

[Serializable]
public struct PerformanceMetrics
{
    public Dictionary<Player,int> singlebuttonHits;
    public int multiplayerButtonHits;
}




public class GameManager : MonoBehaviour
{

    private int exerciseGroupIndex;

    public GameSettings settings;
    private PerformanceMetrics performanceMetrics;

    public GameSceneManager gameSceneManager;

    public GameObject canvas;
    public GameObject playerMarkerPrefab;


    public bool isGameplayPaused;
    public bool isGameplayStarted;

    public bool isButtonOverlap;

    public GameObject namesInputLocation;
    public GameObject playerNameInputFieldPrefabRef;
    
    public GameObject wordPanelsObject;
    public GameObject scorePanelsObject;

    public GameObject timePanel;
    public GameObject track;

    public InputManager inputManager;
    public LogManager logManager;
    
    public AntSpawner[] antSpawners;
    public LetterSpawner[] letterSpawners;

    public List<Button> gameButtons;

    //public Dictionary<Player, Exercise> currExercises;
    //public Dictionary<Player, string> currWordStates;

    private float timeLeft;

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

    public string OrderedFormOfNumber(int i)
    {
        string suffix = "th";
        switch (i)
        {
            case 1:
                suffix = "st";
                break;
            case 2:
                suffix = "nd";
                break;
            case 3:
                suffix = "rd";
                break;
        }
        return i + suffix;
    }
    private void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


    private void UpdateButtonColors()
    {
        //gameButtons.ForEach(delegate (Button button) { button.GetComponent<Image>().color = Color.white; });
        //for(int i=0; i< gameButtons.Count; i++)
        //{
        //    gameButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/button");
        //    int numActivePlayers = 0;
        //    foreach(Player player in settings.players)
        //    {
        //        int activeIndex = player.GetActivebuttonIndex();
        //        if (activeIndex == i)
        //        {
        //            gameButtons[activeIndex].GetComponent<Image>().color = player.GetButtonColor();
        //            numActivePlayers++;
        //        }
        //    }
        //    if (numActivePlayers > 1)
        //    {
        //        gameButtons[i].GetComponent<Image>().color = Color.white;
        //        gameButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/mixedButton");
        //    }
        //}
        foreach(Player player in settings.players)
        {
            foreach(GameObject obj in player.GetMaskedHalf())
            {
                obj.SetActive(!isButtonOverlap);
            }
        }
    }


    private IEnumerator YieldedStart()
    { 
        //currExercises = new Dictionary<Player, Exercise>();
        //currWordStates = new Dictionary<Player, string>();


        isGameplayPaused = false;
        gameSceneManager.MainSceneLoadedNotification();

        //settings = new GameSettings();

        //settings.maxSimultaneousKeyPresses = 2;

        //settings.players = new List<Player>();
        //settings.players.Add(new Player(new List<KeyCode>() { KeyCode.Q, KeyCode.W, KeyCode.E }, new List<string> { "YButtonJoy1", "BButtonJoy1" }));
        //settings.players.Add(new Player(new List<KeyCode>() { KeyCode.I, KeyCode.O, KeyCode.P }, new List<string> { "YButtonJoy2", "BButtonJoy2" }));
        ////players.Add(new Player(Utilities.PlayerId.PLAYER_2, new KeyCode[] { KeyCode.V, KeyCode.B, KeyCode.N }, new string[] { "YButtonJoy3" , "BButtonJoy3" }));


        //settings.exercises = new List<Exercise>();
        //settings.exercises.Add(new Exercise("Fck yourself:_", "HFESUIHFUESIGHUFISEHUFESIHFESI"));
        ////exercises.Add(new Exercise("Word to match: CAKE \n Your Word:_", "CAKE"));
        ////exercises.Add(new Exercise("Word to match: BANANA \n Your Word:_", "BANANA"));
        ////exercises.Add(new Exercise("Word to match: PIE \n Your Word:_", "PIE"));
        ////exercises.Add(new Exercise("Word to match: PIZZA \n Your Word:_", "PIZZA"));
        ////exercises.Add(new Exercise("Word to match: CROISSANT \n Your Word:_", "CROISSANT"));
        ////exercises.Add(new Exercise("Word to match: DONUT \n Your Word:_", "DONUT"));
        ////exercises.Add(new Exercise("Word to match: CHERRY \n Your Word:_", "CHERRY"));
        ////exercises.Add(new Exercise("Word to match: XMASCOOKIES \n Your Word:_", "XMASCOOKIES"));
        ////exercises.Add(new Exercise("Word to match: KIWI \n Your Word:_", "KIWI"));
        ////exercises.Add(new Exercise("Word to match: QUICHE \n Your Word:_", "QUICHE"));
        ////exercises.Add(new Exercise("Word to match: MANGO \n Your Word:_", "MANGO"));
        ////exercises.Add(new Exercise("Word to match: FISH \n Your Word:_", "FISH"));
        ////exercises.Add(new Exercise("Word to match: VANILLA \n Your Word:_", "VANILLA"));
        ////exercises.Add(new Exercise("Word to match: JELLY \n Your Word:_", "JELLY"));


        string path = Application.streamingAssetsPath + "/config.cfg";
        string configText = "";
        if (path.Contains("://") || path.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();
            configText = www.downloadHandler.text;
        }
        else
        {
            configText = File.ReadAllText(path);
        }

        //string json = JsonUtility.ToJson(settings, true);
        settings = JsonUtility.FromJson<GameSettings>(configText);

        //Application.targetFrameRate = 60;

        timeLeft = settings.initialTimeLeft;
        timePanel.GetComponent<Animator>().StopPlayback();

        InvokeRepeating("TimeDependentEvents", 0.0f, 1.0f);

       
        gameSceneManager.StartAndPauseGame(); //for the initial screen


        performanceMetrics = new PerformanceMetrics();
        performanceMetrics.singlebuttonHits = new Dictionary<Player, int>();

        for (int i = 0; i < settings.players.Count; i++)
        {
            Player currPlayer = settings.players[i];
            
            currPlayer.Init(this, playerMarkerPrefab, canvas, wordPanelsObject.transform.GetChild(i).gameObject, scorePanelsObject.transform.GetChild(i).gameObject, (i%2==0));

            currPlayer.GetWordPanel().transform.Find("panel/Layout").GetComponent<Image>().color = currPlayer.GetButtonColor();


            GameObject newNameInput = Instantiate(playerNameInputFieldPrefabRef, namesInputLocation.transform);
            InputField input = newNameInput.GetComponent<InputField>();
            input.placeholder.GetComponent<Text>().text = OrderedFormOfNumber(i + 1) + " player name";
            
            input.onValueChanged.AddListener(delegate
            {
                List<InputField> inputFields = new List<InputField>(namesInputLocation.GetComponentsInChildren<InputField>());
                settings.players[inputFields.IndexOf(input)].SetName(input.text);
            });

            performanceMetrics.singlebuttonHits.Add(currPlayer, 0);

            List<KeyCode> keys = currPlayer.GetMyKeys();

            //UpdateButtonColors();

            isButtonOverlap = true;
            for (int j = 1; j < Globals.keyInteractionTypes.Length ; j++)
            {
                Globals.KeyInteractionType currInt = (Globals.KeyInteractionType) j;
                inputManager.AddKeyBinding(
                    new List<KeyCode>(){ keys[j] }, InputManager.ButtonPressType.PRESSED, delegate (List<KeyCode> triggeredKeys)
                    {
                        currPlayer.SetActiveInteraction(currInt);
                        int activeIndex = currPlayer.GetActivebuttonIndex();
                        //gameButtons[activeIndex].RegisterButtonPress(currPlayer);//Utilities.interactionTypes[j]);
                        currPlayer.GetMarker().GetComponentInChildren<Button>().RegisterButtonPress(currPlayer);
                    }, false);
            }
            inputManager.AddKeyBinding(
                    new List<KeyCode>() { keys[gameButtons.Count] }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys)
                    {
                        int potentialIndex = (currPlayer.GetActivebuttonIndex() - 1);
                        potentialIndex = (potentialIndex < 0)? 0 : potentialIndex;
                        isButtonOverlap = false;
                        foreach (Player player in settings.players)
                        {
                            if (player != currPlayer && player.GetActivebuttonIndex() == potentialIndex)
                            {
                                isButtonOverlap = true;
                                break;
                            }
                        }
                        Debug.Log("Key: " + keys[gameButtons.Count] + "; Potential Index: " + potentialIndex);
                        currPlayer.SetActiveButton(potentialIndex, gameButtons[potentialIndex].transform.position);
                        UpdateButtonColors();
                    }, false);
            inputManager.AddKeyBinding(
                    new List<KeyCode>() { keys[gameButtons.Count + 1] }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys)
                    {
                        int potentialIndex = (currPlayer.GetActivebuttonIndex() + 1);
                        potentialIndex = (potentialIndex > (gameButtons.Count - 1)) ? (gameButtons.Count - 1) : potentialIndex;
                        isButtonOverlap = false;
                        foreach (Player player in settings.players)
                        {
                            if (player != currPlayer && player.GetActivebuttonIndex() == potentialIndex)
                            {
                                isButtonOverlap = true;
                                break;
                            }
                        }
                        Debug.Log("Key: " + keys[gameButtons.Count] + "; Potential Index: " + potentialIndex);
                        currPlayer.SetActiveButton(potentialIndex, gameButtons[potentialIndex].transform.position);
                        UpdateButtonColors();
                    }, false);

            currPlayer.SetActiveButton(0, gameButtons[0].transform.position);
            currPlayer.SetScore(0);
        }
        UpdateButtonColors();

        ChangeGameParametrizations(true);
        

        performanceMetrics.multiplayerButtonHits = 0;
        isGameplayStarted = true;

        exerciseGroupIndex = UnityEngine.Random.Range(0, settings.exercisesGroups.Count);
        ChangeGameParametrizations(true);

        
        inputManager.AddKeyBinding(new List<KeyCode> { KeyCode.Space }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys) { gameSceneManager.StartAndPauseGame(); }, false);

        logManager = new MongoDBLogManager();
        logManager.InitLogs(this);

        Shuffle<ExercisesListWrapper>(settings.exercisesGroups);


    }

    // Use this for initialization
    void Start()
    {
        isGameplayStarted = false;
        StartCoroutine(YieldedStart());
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log("multi: " + performanceMetrics.multiplayerButtonHits);
        if (isGameplayPaused || !isGameplayStarted)
        {
            return;
        }

        ////if no lifes end game immediately
        //if (lifes < 1)
        //{
        //    gameSceneManager.EndGame();
        //}
        
    }

    private void RecordMetrics()
    {
        //spawn questionnaires before changing word
        foreach (Player player in settings.players)
        {
            ////record metrics
            //int totalButtonHits = performanceMetrics.singlebuttonHits[player] + performanceMetrics.multiplayerButtonHits;
            //float playerHitsPercentage = ((float)performanceMetrics.singlebuttonHits[player] / (float)totalButtonHits) * 100.0f;
            //float simultaneousHitsPercentage = ((float)performanceMetrics.multiplayerButtonHits / (float)totalButtonHits) * 100.0f;
            ////StartCoroutine(logManager.WriteToLog("behavioralchangingcrossantlogs", "logs", new Dictionary<string, string>() {
            ////    { "playerName", player.GetName() },
            ////    { "currWord", currWordState },
            ////    { "playerCollPercentage", simultaneousHitsPercentage.ToString() },
            ////    { "playerCompPercentage", simultaneousHitsPercentage.ToString() },
            ////    { "playerSelfPercentage", playerHitsPercentage.ToString() }
            ////}));
            //StartCoroutine(logManager.WriteToLog("behavioralchangingcrossantlogs", "logs", new Dictionary<string, string>() {
            //    { "playerName", player.GetName() },
            //    { "currWord", currWordStates[player] },
            //    { "playerHitsPercentage", playerHitsPercentage.ToString() },
            //    { "simultaneousHitsPercentage", simultaneousHitsPercentage.ToString() }
            //}));
        }
        
    }

    private void PoppupQuestionnaires()
    {
        //gameSceneManager.PauseForQuestionnaires();
        ////spawn questionnaires before changing word
        //foreach (Player player in settings.players)
        //{
        //    Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSeM3Xn5qDBdX7QCtyrPILLbqpYj3ueDcLa_-9CbxCPzxVsMzg/viewform?usp=pp_url&entry.100873100=" + player.GetName()); //spawn questionaires
        //}
    }

    void ChangeLevel()
    {
        RecordMetrics();
        ChangeGameParametrizations(false);
    }

    void TimeDependentEvents()
    {
        Color panelColor = Color.white;
        Color panelTextColor = Color.white;
        if (timeLeft > 1){
            timeLeft--;

            if(timeLeft < 10)
            {
                timePanel.GetComponent<AudioSource>().Play();
                timePanel.GetComponent<Animator>().Play(0);
                panelColor = Color.red;
                panelTextColor = Color.white;
            }
            else
            {
                timePanel.GetComponent<AudioSource>().Stop();
                timePanel.GetComponent<Animator>().StopPlayback();
            }
        }
        else
        {
            ChangeLevel();
            timeLeft = settings.initialTimeLeft;
        }
        Text timePanelText = timePanel.GetComponentInChildren<Text>();
        Image timePanelImage = timePanel.GetComponentInChildren<Image>();
        timePanelImage.color = panelColor;
        timePanelText.color = panelTextColor;
        timePanelText.text =  (int)((timeLeft/ settings.initialTimeLeft)*100) + " %";
    }

    private void OnApplicationQuit()
    {
        //RecordMetrics();
        //PoppupQuestionnaires();
    }
    

    private void ChangeGameParametrizations(bool firstTimeCall)
    {
        for(int i=0; i<letterSpawners.Length; i++)
        {
            if(!firstTimeCall && letterSpawners[i].minIntervalRange > 0.3 && letterSpawners[i].maxIntervalRange > 0.4)
            {
                letterSpawners[i].minIntervalRange -= 0.1f;
                letterSpawners[i].maxIntervalRange -= 0.1f;
            }
        }

        for (int i = 0; i < letterSpawners.Length; i++)
        {
            letterSpawners[i].UpdateCurrStarredWord("");
        }

        ChangeTargetWords();

    }
    
    private void ChangeTargetWords()
    {
        List<Exercise> selectedExerciseGroup = new List<Exercise>(settings.exercisesGroups[exerciseGroupIndex++ % settings.exercisesGroups.Count].exercises);
        foreach (Player player in settings.players)
        {
            if(selectedExerciseGroup.Count <= 0)
            {
                selectedExerciseGroup = new List<Exercise>(settings.exercisesGroups[exerciseGroupIndex++ % settings.exercisesGroups.Count].exercises);
            }

            int random = UnityEngine.Random.Range(0, selectedExerciseGroup.Count);
            Exercise newExercise = selectedExerciseGroup[random];
            selectedExerciseGroup.RemoveAt(random);
            
            
            //displayPanel.GetComponent<DisplayPanel>().SetTargetImage(newExercise.targetWord);
            player.SetCurrExercise(newExercise);
            player.SetCurrWordState("");

            //animate transition
            player.GetWordPanel().GetComponentInChildren<Animator>().Play(0);
        }
        gameObject.GetComponent<AudioSource>().Play();
    }


    private bool TestAndExecuteHit(bool execute, char letterText, GameObject letter, Player player)
    {
        string currWordState = player.GetCurrWordState() + letterText;
        string currTargetWord = player.GetCurrExercise().targetWord;

        //check the utility of word
        bool usefulForMe = (currWordState.Length <= currTargetWord.Length && currTargetWord[currWordState.Length - 1] == currWordState[currWordState.Length - 1]);

        if (execute && usefulForMe)
        {
            player.SetCurrWordState(player.GetCurrWordState() + letterText);
            letter.GetComponent<Letter>().isTranslationEnabled = false;
            StartCoroutine(Globals.LerpAnimation(letter, player.GetWordPanel().transform.position, 1.0f));
        }

        return usefulForMe;
    }

    public void RecordHit(char letterText, GameObject letter, HashSet<Player> currHitters)
    {
        
        foreach (Player player in currHitters)
        {
            bool usefulForMe = false;
            bool usefulForOther = false;

            //diferent rewards in different utility conditions
            Globals.KeyInteractionType playerIT = player.GetActiveInteraction();
            List<ScoreValue> scores = new List<ScoreValue>();
            switch (playerIT)
            {
                case Globals.KeyInteractionType.GIVE:
                    usefulForMe = TestAndExecuteHit(false, letterText, letter, player);
                    foreach (Player usefulTargetPlayer in settings.players)
                    {
                        if (usefulTargetPlayer == player)
                        {
                            continue;
                        }
                        usefulForOther = TestAndExecuteHit(true, letterText, letter, usefulTargetPlayer);
                        if (usefulForOther)
                        {
                            letter.GetComponent<Image>().color = player.GetButtonColor();
                            break;
                        }

                    }
                    scores = settings.scoreSystem.giveScores;
                    break;
                case Globals.KeyInteractionType.TAKE:
                    usefulForMe = TestAndExecuteHit(true, letterText, letter, player);
                    if (usefulForMe)
                    {
                        letter.GetComponent<Image>().color = player.GetButtonColor();
                    }
                    foreach (Player usefulTargetPlayer in settings.players)
                    {
                        if (usefulTargetPlayer == player)
                        {
                            continue;
                        }
                        if (usefulForOther)
                        {
                            break;
                        }

                        usefulForOther = TestAndExecuteHit(false, letterText, letter, usefulTargetPlayer);
                    }
                    scores = settings.scoreSystem.takeScores;
                    break;
            }

            foreach (ScoreValue score in scores)
            {
                if (score.usefulForMe == usefulForMe && score.usefulForOther == usefulForOther)
                {
                    player.SetScore(player.GetScore() + score.myValue);
                    foreach (Player innerPlayer in settings.players)
                    {
                        if (innerPlayer == player)
                        {
                            continue;
                        }
                        innerPlayer.SetScore(innerPlayer.GetScore() + score.otherValue);
                    }
                    break;
                }
            }

            
        }

        bool areWordsUnfinished = false;
        foreach (Player player in settings.players)
        {
            string currWordState = player.GetCurrWordState();
            string currTargetWord = player.GetCurrExercise().targetWord;
            if (currWordState.CompareTo(currTargetWord) != 0)
            {
                if(!areWordsUnfinished)
                    areWordsUnfinished = true;

                if (!player.isCurrExerciseFinished)
                    player.isCurrExerciseFinished = true;
            }
            else
            {
                if (!player.isCurrExerciseFinished)
                    player.SetScore(player.GetScore() + settings.scoreSystem.completeWordMyScore);
                foreach (Player innerPlayer in settings.players)
                {
                    if (player == innerPlayer)
                    {
                        continue;
                    }
                    if (!innerPlayer.isCurrExerciseFinished)
                        innerPlayer.SetScore(innerPlayer.GetScore() + settings.scoreSystem.completeWordOtherScore);
                }
            }
        }
        if (!areWordsUnfinished)
        {
            //foreach (AntSpawner antSpawner in antSpawners)
            //{
            //    antSpawner.SpawnAnt(currTargetWord);
            //}
            ChangeLevel();
        }

    }

    public List<Player> GetPlayers()
    {
        return settings.players;
    }
}


