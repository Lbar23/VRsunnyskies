using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    public void StartNormalMode()
    {
        SceneManager.LoadScene("Task4");  // Loads the task4 scene
    }

    public void QuitGame()
    {
        Application.Quit();  // Exits the application
    }
}
