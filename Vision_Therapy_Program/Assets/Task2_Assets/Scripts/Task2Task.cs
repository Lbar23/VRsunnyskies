using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task2Task : MonoBehaviour
{
    // The time to wait before moving the ball
    public float waitTime = 5f;

    // The amount to move the ball along the X-axis
    public float moveAmount = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Start the move operation after the specified wait time
        Invoke("MoveBallX", waitTime);
    }

    // Function to move the ball's X position
    void MoveBallX()
    {
        // Move the ball by 1 unit along the X axis
        transform.position += new Vector3(moveAmount, 0f, 0f);
    }
}
