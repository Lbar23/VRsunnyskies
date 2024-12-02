using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownScript : MonoBehaviour
{
    public Text countdownText;   // Reference to the UI Text component
    public int countdownTime = 3; // Countdown starts at 3
    public GameObject gameScene;  // Optional: A reference to the game scene you want to activate after countdown
    
    private void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        int timeLeft = countdownTime;

        while (timeLeft > 0)
        {
            countdownText.text = timeLeft.ToString(); // Update UI Text with the current number
            yield return new WaitForSeconds(1);       // Wait for 1 second
            timeLeft--;                               // Decrease the number
        }

        countdownText.text = "Go!"; // After the countdown reaches 0, display "Go!"
        yield return new WaitForSeconds(1); // Optional delay before hiding or transitioning scenes
        
        // Hide the countdown text or start the game
        countdownText.gameObject.SetActive(false);
        
        // Optional: If you want to load the game scene after the countdown
        if (gameScene != null)
        {
            gameScene.SetActive(true);  // Enable the game scene or transition here
        }
    }
}
