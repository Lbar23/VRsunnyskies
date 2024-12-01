using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // For TextMeshPro

public class Task2Task : MonoBehaviour
{
    public GameObject[] balls;                // Array to hold the 3 balls
    public Material[] defaultMaterials;       // Array for the default materials
    public Material[] litMaterials;           // Array for the lit materials
    public Light[] ballLights;                // Array to hold the light components for each ball
    public TextMeshProUGUI timerText;         // Reference to the UI Text component on the wall
    private int currentBallIndex = 0;         // Index to track which ball is currently lit
    private float countdown = 15f;            // 15 seconds initial countdown
    private bool gameStarted = false;         // Boolean to check if the game has started

    void Start()
    {
        // Start the preparation countdown
        StartCoroutine(StartPreparationCountdown());
    }

    // Coroutine to handle the 15-second preparation countdown
    IEnumerator StartPreparationCountdown()
    {
        while (countdown > 0)
        {
            timerText.text = "Get ready: " + countdown.ToString("F0");
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        timerText.text = "Start!";
        yield return new WaitForSeconds(1f);  // Brief pause before starting the main cycle
        StartCoroutine(LightUpRoutine());
    }

    // Coroutine to change the lit ball every 15 seconds
    IEnumerator LightUpRoutine()
    {
        gameStarted = true;
        int checker = 0;

        while (gameStarted)
        {
            ResetBallMaterials();
            LightUpBall(currentBallIndex);

            countdown = 15f;
            while (countdown > 0)
            {
                timerText.text = "Next ball in: " + countdown.ToString("F0");
                yield return new WaitForSeconds(1f);
                countdown--;
            }

            currentBallIndex = (currentBallIndex + 1) % balls.Length;
            checker++;
            Debug.Log(checker);
            if (checker > 10)
            {
                gameStarted = false;
                ResetBallMaterials();
                timerText.text = "Game is finished!";
                yield return StartCoroutine(StartNextTaskCountdown()); // Start next task countdown
            }
        }
    }

    // Coroutine to handle the 5-second countdown for the next task
    IEnumerator StartNextTaskCountdown()
    {
        countdown = 5f;
        while (countdown > 0)
        {
            timerText.text = "Game is finished! Next Task starts in: " + countdown.ToString("F0");
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        // Start the next task (Task4) here
        SceneManager.LoadScene("Task4");

        // Add your Task4 logic here
    }

    // Function to set all balls back to their default materials and turn off lights
    void ResetBallMaterials()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].GetComponent<Renderer>().material = defaultMaterials[i]; // Reset material
            ballLights[i].enabled = false;  // Turn off the light
        }
    }

    // Function to light up the ball by changing its material and turning on its light
    void LightUpBall(int index)
    {
        balls[index].GetComponent<Renderer>().material = litMaterials[index]; // Change material
        ballLights[index].enabled = true;  // Turn on the light
    }
}
