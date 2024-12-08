using UnityEngine;

public class GrabHandler : MonoBehaviour
{
    public DualGrabBar barScript; // Reference to the DualGrabBar script
    public bool isLeftHand;       // True for left grab point, false for right

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            if (isLeftHand)
                barScript.GrabLeft(true);
            else
                barScript.GrabRight(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            if (isLeftHand)
                barScript.GrabLeft(false);
            else
                barScript.GrabRight(false);
        }
    }
}
