using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundPoint : MonoBehaviour
{
    public Transform centerPoint;  // The point to rotate around
    public float speed = 50f;      // Speed of rotation
    public Vector3 rotationAxis = Vector3.forward; // Set axis to Z-axis (0, 0, 1)
    public float duration = 30f;   // Duration of the rotation (in seconds)

    private float elapsedTime = 0f; // Timer to keep track of time

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        // Increment the elapsed time by the time that passed since the last frame
        elapsedTime += Time.deltaTime;

        // Rotate only if the elapsed time is less than the duration (30 seconds)
        if (elapsedTime < duration)
        {
            transform.RotateAround(centerPoint.position, rotationAxis, speed * Time.deltaTime);
        }
    }
}
