using UnityEngine;
using TMPro;
using Oculus.Interaction;
using Oculus.Interaction.HandSphere;

public class TouchScore : MonoBehaviour
{
    public int score = 0; // Initial score
    public int pointsPerTouch = 1; // Points added per touch
    public TextMeshPro scoreText; // Reference to the world-space TextMeshPro object

    private Interactable interactable; // Reference to the Interactable component

    private void Awake()
    {
        // Get the Interactable component attached to this object
        interactable = GetComponent<Interactable>();
    }

    private void Start()
    {
        UpdateScoreDisplay(); // Initialize the score display
    }

    private void OnEnable()
    {
        // Register for the select event when the object is interacted with
        if (interactable != null)
        {
            interactable.WhenSelected.AddListener(OnObjectTouched);
        }
    }

    private void OnDisable()
    {
        // Unregister the event listener to prevent memory leaks
        if (interactable != null)
        {
            interactable.WhenSelected.RemoveListener(OnObjectTouched);
        }
    }

    private void OnObjectTouched(Interactor interactor)
    {
        // Increment score when the object is selected (grabbed/touched)
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
