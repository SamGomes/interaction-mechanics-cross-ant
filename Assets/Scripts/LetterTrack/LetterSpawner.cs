using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LetterSpawner : MonoBehaviour
{
    public GameObject canvas;

    public GameObject letterPrefab;
    public GameManager gameManager;

    public float minIntervalRange;
    public float maxIntervalRange;

    private float randomInterval;

    private string currStarredWord;


    //private char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'L', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
    //"1", "2", "3", "4", "5", "6", "7", "8", "9"};

    private char[] letters = { 'A' };

    private List<char> lettersPool;

    private void Awake()
    {
        lettersPool = new List<char>();
    }

    // Use this for initialization
    void Start()
    {
        float initialDelayInSeconds = 1.0f;
        StartCoroutine(SpawnLetterWithDelay(initialDelayInSeconds));
    }


    public void UpdateCurrStarredWord(string currTargetWord)
    {
        this.currStarredWord = currTargetWord;
    }

    IEnumerator SpawnLetterWithDelay(float sec)
    {

        yield return new WaitForSeconds(sec);
        if (lettersPool.Count == 0)
        {
            ResetPool();
        }
        GameObject newLetter = Instantiate(letterPrefab,canvas.transform);

        Text letterText = newLetter.transform.GetComponentInChildren<Text>();

        int random = Random.Range(0, lettersPool.Count - 1);
        char currLetter = lettersPool[random];
        lettersPool.RemoveAt(random);

        //currLetter = 'A';
        

        newLetter.GetComponent<Letter>().letterText = currLetter;
        if (currStarredWord.Contains(currLetter.ToString().ToUpper()))
        {
            newLetter.GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        //newLetter.GetComponent<Letter>().speed = newLetter.GetComponent<Letter>().speed + ((score + 1) * 0.05f);

        letterText.text =  currLetter.ToString();

        newLetter.transform.position = gameObject.transform.position;
        newLetter.transform.rotation = gameObject.transform.rotation;

        randomInterval = Random.Range(minIntervalRange, maxIntervalRange);
        StartCoroutine(SpawnLetterWithDelay(randomInterval));
    }

    private void ResetPool()
    {
        lettersPool = letters.ToList<char>();
        List<char> currWordLetters = new List<char>();
        foreach(Player player in gameManager.settings.players)
        {
            currWordLetters = currWordLetters.Union(player.GetCurrExercise().targetWord.ToCharArray()).ToList<char>();
        }

        //bias generation of letters in word
        lettersPool.AddRange(currWordLetters);
        lettersPool.AddRange(currWordLetters);
    }


}
