using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class BallPositionTracker : MonoBehaviour
{
    // Public variables
    public GameObject cube1; // First cube to track position
    public GameObject cube2; // Second cube to track position
    public int score = 0; // The player's score

    // Reference to the TextMeshProUGUI component to display score
    public TextMeshProUGUI scoreText;

    // Distance threshold to check if the ball is at the position of the cubes
    public float positionThreshold;

    // Flags to track if the score has already been updated for a cube
    private bool cube1Reached = false;
    private bool cube2Reached = false;

    void Update()
    {
        // Check if the ball is within the threshold distance from cube1 and hasn't already updated the score
        if (!cube1Reached && Vector3.Distance(transform.position, cube1.transform.position) <= positionThreshold)
        {
            // Increase the score by 1
            score++;

            // Log the score to the console (optional)
            Debug.Log("Ball has reached the position of cube1! Score: " + score);

            // Update the score on the UI
            UpdateScore();

            // Set the flag to true to prevent multiple updates
            cube1Reached = true;
        }
        else if (cube1Reached && Vector3.Distance(transform.position, cube1.transform.position) > positionThreshold)
        {
            // Reset the flag if the ball moves away from the cube
            cube1Reached = false;
        }

        // Check if the ball is within the threshold distance from cube2 and hasn't already updated the score
        if (!cube2Reached && Vector3.Distance(transform.position, cube2.transform.position) <= positionThreshold)
        {
            // Increase the score by 1
            score++;

            // Log the score to the console (optional)
            Debug.Log("Ball has reached the position of cube2! Score: " + score);

            // Update the score on the UI
            UpdateScore();

            // Set the flag to true to prevent multiple updates
            cube2Reached = true;
        }
        else if (cube2Reached && Vector3.Distance(transform.position, cube2.transform.position) > positionThreshold)
        {
            // Reset the flag if the ball moves away from the cube
            cube2Reached = false;
        }
    }

    // Method to update the score in the UI
    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}
