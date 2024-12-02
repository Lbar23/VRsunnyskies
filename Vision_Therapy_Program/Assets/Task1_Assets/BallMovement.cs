using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMoveBackAndForth : MonoBehaviour
{
    public Transform pointA;  // First point (assign in Unity Inspector)
    public Transform pointB;  // Second point (assign in Unity Inspector)
    public float speed = 5.0f; // Speed of movement
    private float timer = 0f;  // Timer to track 30 seconds
    private bool movingToB = true; // True if moving to point B, false if moving to point A
    private float duration = 30f;  // Duration for the back and forth movement

    // Start is called before the first frame update
    void Start()
    {
        
    }


    void Update()
    {
        // Increase the timer
        timer += Time.deltaTime;

        // Stop moving after 30 seconds
        if (timer >= duration)
        {
            return;
        }

        // Move the ball back and forth between point A and point B
        if (movingToB)
        {
            MoveTowardsPoint(pointB);
            if (Vector3.Distance(transform.position, pointB.position) < 0.1f)
            {
                movingToB = false;
            }
        }
        else
        {
            MoveTowardsPoint(pointA);
            if (Vector3.Distance(transform.position, pointA.position) < 0.1f)
            {
                movingToB = true;
            }
        }
    }

    // Helper function to move the object towards a target point
    private void MoveTowardsPoint(Transform target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
}
