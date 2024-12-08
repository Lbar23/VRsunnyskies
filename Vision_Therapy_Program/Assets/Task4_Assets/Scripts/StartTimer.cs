using UnityEngine;
using TMPro;

public class StartTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;      // UI element to display the timer
    public float startTimerDuration = 3f;  // Countdown duration in seconds
    public GameObject ball;                // Reference to the ball GameObject

    private float timer;                   // Internal timer variable
    private bool timerFinished = false;    // Flag to indicate if the timer is done

    void Start()
    {
        timer = startTimerDuration;        // Initialize the timer
        UpdateTimerUI();

        if (ball != null)
        {
            ball.SetActive(false);         // Disable the ball initially
        }
    }

    void Update()
    {
        if (!timerFinished)
        {
            timer -= Time.deltaTime;       // Decrease the timer

            if (timer <= 0)
            {
                timer = 0;
                timerFinished = true;
                timerText.text = "";       // Clear the timer display

                if (ball != null)
                {
                    timerText.text = "Score: 0";
                    ball.SetActive(true);  // Enable the ball when the timer ends
                }
            }

            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "The task will begin in: " + Mathf.Ceil(timer).ToString(); // Update UI text
        }
    }
}
