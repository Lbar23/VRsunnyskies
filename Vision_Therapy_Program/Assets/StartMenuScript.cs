using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    public void StartNormalMode()
    {
        PlayerPrefs.SetInt("NextTutorial", 5);
        PlayerPrefs.SetInt("TutorialMode", 0);
        SceneManager.LoadScene("Task5");
    }

    public void StartTutorialMode()
    {
        PlayerPrefs.SetInt("TutorialMode", 1);
        PlayerPrefs.SetInt("NextTutorial", 5);
        SceneManager.LoadScene("TutorialPlayer");
    }

    public void QuitGame()
    {
        Application.Quit();  // Exits the application
    }
}
