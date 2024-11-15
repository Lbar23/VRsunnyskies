using UnityEngine;
using UnityEngine.EventSystems;

public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer; // The laser beam
    public Transform laserOrigin;     // Where the laser originates (e.g., the controller)

    public float laserLength = 5.0f;  // Maximum laser distance
    public LayerMask interactionMask; // Layer to detect UI elements

    void Update()
    {
        UpdateLaser();
    }

    private void UpdateLaser()
    {
        // Raycast from the controller to detect objects
        Ray ray = new Ray(laserOrigin.position, laserOrigin.forward);
        RaycastHit hit;

        // Check if the laser hits something in the interaction mask
        if (Physics.Raycast(ray, out hit, laserLength, interactionMask))
        {
            lineRenderer.SetPosition(1, new Vector3(0, 0, hit.distance)); // Adjust length
            // Optionally: Highlight or interact with the hit object
        }
        else
        {
            lineRenderer.SetPosition(1, new Vector3(0, 0, laserLength)); // Default length
        }
    }
}
