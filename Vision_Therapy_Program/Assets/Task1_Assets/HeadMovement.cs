using UnityEngine;
using UnityEngine.UI; // For displaying the message
using UnityEngine.XR; // For accessing XR device input

public class HeadMovement : MonoBehaviour
{
    public Text warningText;         // UI Text to display the warning message
    public float maxDistance = 0.5f; // Maximum distance before showing the warning

    private Vector3 startingPosition; // The original head position

    void Start()
    {
        // Get the starting position of the VR headset
        startingPosition = InputTracking.GetLocalPosition(XRNode.Head);
        
        // Ensure the warning text is not shown initially
        if (warningText != null)
        {
            warningText.enabled = false;
        }
    }

    void Update()
    {
        // Get the current head position
        Vector3 currentHeadPosition = InputTracking.GetLocalPosition(XRNode.Head);

        // Calculate the distance between the current position and the starting position
        float distance = Vector3.Distance(startingPosition, currentHeadPosition);

        // Check if the head has moved too far
        if (distance > maxDistance)
        {
            // Show the warning message
            if (warningText != null)
            {
                warningText.text = "You've moved too far!";
                warningText.enabled = true;
            }
        }
        else
        {
            // Hide the warning message if the head is within the allowed distance
            if (warningText != null)
            {
                warningText.enabled = false;
            }
        }
    }
}
