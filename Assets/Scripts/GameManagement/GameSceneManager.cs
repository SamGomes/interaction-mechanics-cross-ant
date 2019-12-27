using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour {

    private bool isGamePaused;
    private bool isGameLoaded;
    public GameManager gameManager;

    private GameObject currPauseMenu;
    private GameObject plainPauseMenu;
    private GameObject questionnairesMenu;
    private GameObject initialConfigMenu;

    public void StartAndPauseGame(Utilities.PlayerId caller)
    {
        if (!isGameLoaded)
        {
            this.StartGame(caller);
        }
        else
        {
            this.PauseGame(caller);
        }
    }

    public void MainSceneLoadedNotification()
    {
        isGameLoaded = true;
        this.plainPauseMenu = GameObject.Find("Canvas/PauseCanvas");
        this.questionnairesMenu = GameObject.Find("Canvas/QuestionnairePauseCanvas");
        this.initialConfigMenu = GameObject.Find("Canvas/InitialConfigPauseCanvas");
        plainPauseMenu.SetActive(false);
        questionnairesMenu.SetActive(false);
        initialConfigMenu.SetActive(false);
        this.currPauseMenu = initialConfigMenu;
    }

    private void StartGame(Utilities.PlayerId caller)
    {
        //set stuff
        isGamePaused = false;
        isGameLoaded = false;

        gameObject.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene("crossAnt");
    }
    public void EndGame()
    {
        //reset stuff
        isGamePaused = false;
        isGameLoaded = false;

        SceneManager.LoadScene("gameover");
    }

    private void PauseGame(Utilities.PlayerId caller)
    {
        if (!isGamePaused)
        {
            gameManager.PauseGame();
            currPauseMenu.SetActive(true);
        }
        else
        {
            gameManager.ResumeGame();
            currPauseMenu.SetActive(false);

            //regain pause menu everytime user unpauses the questionnaire menu
            if (currPauseMenu != plainPauseMenu)
            {
                currPauseMenu = plainPauseMenu;
            }
        }
        isGamePaused = !isGamePaused;
    }

    public void quitGame(Utilities.PlayerId caller)
    {
        Application.Quit();
    }

    public void pauseForQuestionnaires(Utilities.PlayerId caller)
    {
        this.currPauseMenu = questionnairesMenu;
        this.PauseGame(caller);
    }
}
