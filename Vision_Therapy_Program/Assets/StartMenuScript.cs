using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{

    public void Start(){
        Logger.Log("");
        Logger.Log("Vision Therapy Programm starting");
    }

    public void StartNormalMode()
    {
        PlayerPrefs.SetInt("TutorialMode", 0);
        SceneManager.LoadScene("Task5");
    }

    public void StartTutorialMode()
    {
        PlayerPrefs.SetInt("TutorialMode", 1);
        PlayerPrefs.SetInt("NextTutorial", 1);
        SceneManager.LoadScene("TutorialPlayer");
    }

    public void QuitGame()
    {
        Application.Quit();  // Exits the application
    }
}
