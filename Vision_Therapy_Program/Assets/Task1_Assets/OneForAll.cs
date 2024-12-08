using UnityEngine;
using UnityEngine.SceneManagement;

public class OneForAll : MonoBehaviour
{
    public GameObject[] objects;        // Array to hold the 5 objects
    public float showDuration = 2f;     // Duration each object is shown (in seconds)
    public float endAfterSeconds = 10f; // Total time after which the movement should stop

    private int currentIndex = 0;       // Index of the current object being shown
    private float timer = 0f;           // Timer to keep track of time
    private bool movementStopped = false; // Flag to check if the movement has been stopped

    void Start()
    {
        // Make sure all objects are initially inactive
        HideAllObjects();

        // Start by showing the first object
        if (objects.Length > 0)
            objects[currentIndex].SetActive(true);
    }

    private void JumpToNextTask()
    {
        if (PlayerPrefs.GetInt("TutorialMode", 0) == 1)
        {
            PlayerPrefs.SetInt("NextTutorial", 2);
            SceneManager.LoadScene("TutorialPlayer");
        }
        else
        {
            SceneManager.LoadScene("Task2");
        }
    }

    void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;

        // Check if it's time to stop the movement
        if (timer >= endAfterSeconds && !movementStopped)
        {
            // Stop the movement of objects by setting the flag
            movementStopped = true;

            // Optionally, you can stop showing objects
            HideAllObjects();

            JumpToNextTask();
            return; // Exit the Update loop to stop the logic
        }

        // Continue showing objects if the movement hasn't been stopped
        if (!movementStopped)
        {
            // If the timer exceeds the show duration, move to the next object
            if (timer % showDuration < Time.deltaTime) // Time to change object
            {
                // Hide the current object
                objects[currentIndex].SetActive(false);

                // Move to the next object (looping back to the start if necessary)
                currentIndex = (currentIndex + 1) % objects.Length;

                // Show the next object
                objects[currentIndex].SetActive(true);
            }
        }
    }

    // Function to hide all objects
    void HideAllObjects()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }
}
