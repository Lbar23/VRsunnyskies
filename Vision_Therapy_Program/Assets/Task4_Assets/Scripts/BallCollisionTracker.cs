using UnityEngine;
using TMPro;  // Import TextMeshPro namespace
using UnityEngine.SceneManagement;  // Import for scene management

public class BallPositionTracker : MonoBehaviour
{
    // Public variables
    public GameObject cube1;  // First cube to track position
    public GameObject cube2;  // Second cube to track position
    public GameObject respawn;
    public TextMeshProUGUI scoreText;  // Reference to TextMeshProUGUI to display score
    public float positionThreshold;  // Distance threshold for score update

    public int score = 0;  // The player's score
    private bool cube1Reached = false;  // Flag for cube1 scoring
    private bool cube2Reached = false;  // Flag for cube2 scoring
    private bool taskComplete = false;  // Flag to stop scoring after task completion

    void Update()
    {
        if (taskComplete) return;  // Exit if task is already complete

        // Check if ball is near cube1
        if (!cube1Reached && Vector3.Distance(transform.position, cube1.transform.position) <= positionThreshold)
        {
            score++;
            Debug.Log("Ball has reached the position of cube1! Score: " + score);
            UpdateScore();
            cube1Reached = true;
        }
        else if (cube1Reached && Vector3.Distance(transform.position, cube1.transform.position) > positionThreshold)
        {
            cube1Reached = false;
        }

        // Check if ball is near cube2
        if (!cube2Reached && Vector3.Distance(transform.position, cube2.transform.position) <= positionThreshold)
        {
            score++;
            Debug.Log("Ball has reached the position of cube2! Score: " + score);
            UpdateScore();
            cube2Reached = true;
        }
        else if (cube2Reached && Vector3.Distance(transform.position, cube2.transform.position) > positionThreshold)
        {
            cube2Reached = false;
        }

        // Respawn ball if it falls below y = 1
        if (transform.position.y <= 1)
        {
            transform.position = respawn.transform.position;
        }
    }

    // Method to update the score in the UI
    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }

        // Check if score reaches 10
        if (score >= 10)
        {
            TaskComplete();
        }
    }

    // Method called when the score reaches 10
    private void TaskComplete()
    {
        taskComplete = true;  // Prevent further scoring
        StartCoroutine(ShowTaskCompleteMessage());
    }

    // Coroutine to show the "Task Complete" message and change scenes
    private System.Collections.IEnumerator ShowTaskCompleteMessage()
    {
        float countdown = 5f;  // Countdown duration in seconds

        while (countdown > 0)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Task Complete! Next task in: {Mathf.Ceil(countdown)}";
            }
            countdown -= Time.deltaTime;  // Decrement countdown
            yield return null;  // Wait for the next frame
        }

        // Load the next scene after countdown
        SceneManager.LoadScene("Task5");
    }
}
