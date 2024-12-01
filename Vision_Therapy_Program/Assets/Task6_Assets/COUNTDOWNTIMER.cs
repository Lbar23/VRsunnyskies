using UnityEngine;
using TMPro;

public class WorldCountdownTimer : MonoBehaviour
{
    public float timeRemaining = 60; // 1-minute countdown in seconds
    public TextMeshPro worldTimerText;

    private void Start()
    {
        UpdateTimerDisplay(); // Display initial time
    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else
        {
            timeRemaining = 0; // Ensure the timer stops at 0
            UpdateTimerDisplay();
            TimerEnded();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        worldTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void TimerEnded()
    {
        // Code to execute when the timer hits 0, if any
        Debug.Log("Timer ended!");
    }
}
