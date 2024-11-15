using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class TouchScore : MonoBehaviour
{
    public int score = 0; // Initial score
    public int pointsPerTouch = 1; // Points added per touch
    public TextMeshPro scoreText; // Reference to the world-space TextMeshPro object

    private void Start()
    {
        UpdateScoreDisplay(); // Initialize the score display
    }

    private void OnEnable()
    {
        // Register for the select event
        GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnObjectTouched);
    }

    private void OnDisable()
    {
        // Unregister to prevent memory leaks
        GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(OnObjectTouched);
    }

    private void OnObjectTouched(SelectEnterEventArgs args)
    {
        // Increment score when the object is touched/grabbed
        score += pointsPerTouch;
        UpdateScoreDisplay(); // Update the score display
    }

    private void UpdateScoreDisplay()
    {
        // Set the score text in the world
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
