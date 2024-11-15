using UnityEngine;

public class DualGrabBar : MonoBehaviour
{
    public Transform leftGrab; // Reference to the left grab point
    public Transform rightGrab; // Reference to the right grab point

    private bool isLeftGrabbing = false;
    private bool isRightGrabbing = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Keep the bar controllable without external physics
    }

    void FixedUpdate()
    {
        // If both hands are grabbing, compute the midpoint and adjust orientation
        if (isLeftGrabbing && isRightGrabbing)
        {
            Vector3 midpoint = (leftGrab.position + rightGrab.position) / 2;
            rb.MovePosition(midpoint);

            Vector3 direction = rightGrab.position - leftGrab.position;
            rb.MoveRotation(Quaternion.LookRotation(direction, Vector3.up));
        }
        // If only the left hand is grabbing
        else if (isLeftGrabbing)
        {
            rb.MovePosition(leftGrab.position);
        }
        // If only the right hand is grabbing
        else if (isRightGrabbing)
        {
            rb.MovePosition(rightGrab.position);
        }
    }

    public void GrabLeft(bool isGrabbing)
    {
        isLeftGrabbing = isGrabbing;
    }

    public void GrabRight(bool isGrabbing)
    {
        isRightGrabbing = isGrabbing;
    }
}
