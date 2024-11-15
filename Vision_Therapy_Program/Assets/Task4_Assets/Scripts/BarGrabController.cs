using UnityEngine;

public class BarGrabController : MonoBehaviour
{
    public Transform endLeft;
    public Transform endRight;
    public Transform bar;

    private bool isLeftGrabbed = false;
    private bool isRightGrabbed = false;

    private Vector3 initialBarPosition;
    private Quaternion initialBarRotation;

    void Start()
    {
        initialBarPosition = bar.position;
        initialBarRotation = bar.rotation;
    }

    void Update()
    {
        // Check if either end is grabbed
        isLeftGrabbed = endLeft.GetComponent<OVRGrabbable>().isGrabbed;
        isRightGrabbed = endRight.GetComponent<OVRGrabbable>().isGrabbed;

        if (isLeftGrabbed || isRightGrabbed)
        {
            // Move the bar based on the relative positions of the ends
            Vector3 midpoint = (endLeft.position + endRight.position) / 2;
            bar.position = midpoint;

            // Calculate rotation based on the vector between the ends
            Vector3 direction = endRight.position - endLeft.position;
            bar.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);
        }
        else
        {
            // Reset to the initial position and rotation if nothing is grabbed
            bar.position = initialBarPosition;
            bar.rotation = initialBarRotation;
        }
    }
}
