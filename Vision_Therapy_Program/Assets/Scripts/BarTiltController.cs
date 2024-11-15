using UnityEngine;
using UnityEngine.XR;

public class BarTiltController : MonoBehaviour
{
    public float rotationSpeed = 10f; // Speed of rotation

    // Input device references
    private InputDevice leftController;
    private InputDevice rightController;

    private void Start()
    {
        // Get input devices for left and right controllers
        leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private void Update()
    {
        // Check if we have valid devices
        if (!leftController.isValid)
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (!rightController.isValid)
            rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Read trigger inputs
        float leftTriggerValue = 0f;
        float rightTriggerValue = 0f;

        leftController.TryGetFeatureValue(CommonUsages.trigger, out leftTriggerValue);
        rightController.TryGetFeatureValue(CommonUsages.trigger, out rightTriggerValue);

        // Calculate rotation
        float rotationInput = (leftTriggerValue - rightTriggerValue) * rotationSpeed * Time.deltaTime;

        // Apply rotation to the bar
        transform.Rotate(Vector3.right, rotationInput);
    }
}
