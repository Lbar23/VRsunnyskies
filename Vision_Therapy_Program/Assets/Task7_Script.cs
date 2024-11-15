using UnityEngine;
using System.Collections.Generic;
using Oculus.Platform;
using Unity.VisualScripting;
using System.Collections;
using TMPro;
using Oculus.Interaction;
using Oculus.Interaction.Grab;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.UnityCanvas;

public class RandomObjectPlacer : MonoBehaviour
{
    public List<GameObject> allObjects; // Assign individual objects in the Inspector
    public GameObject table; // Assign the table GameObject
    public TextMeshPro distanceTextOriginal;

    public int minObjects = 3;
    public int maxObjects = 6;

    public OVRHand rightHand;
    public OVRHand leftHand;

    private Vector3 tableCenter;
    private Vector3 tableSize;
    private float tableWidth;
    private float tableTopY;
    private float marginX = 0;
    private float marginZ = 0;

    private static Vector3 textOffset = new Vector3(0, 0.05f, 0);

    private float totalDistance = 0f;
    private int totalMeasurements = 0;

    void Start()
    {
        Initialize();
        StartCoroutine(StartTask7());
    }

    private void Initialize()
    {
        Collider tableCollider = table.GetComponent<Collider>();

        if (tableCollider != null)
        {
            // Add this line to ensure the table's collider isn't a trigger
            tableCollider.isTrigger = false;

            // Use Collider bounds
            tableCenter = tableCollider.bounds.center;
            tableSize = tableCollider.bounds.size;
            tableWidth = tableCollider.bounds.size.x;
            tableTopY = tableCollider.bounds.max.y; // Top surface Y position
        }
        else
        {
            // Fallback to Renderer bounds if Collider is not available
            Renderer tableRenderer = table.GetComponent<Renderer>();
            if (tableRenderer != null)
            {
                tableCenter = tableRenderer.bounds.center;
                tableSize = tableRenderer.bounds.size;
                tableWidth = tableCollider.bounds.size.x;
                tableTopY = tableRenderer.bounds.max.y; // Top surface Y position
            }
        }

        marginX = tableSize.x * 0.2f / 2;
        marginZ = tableSize.z * 0.2f / 2;
    }

    private IEnumerator WaitForUserInput()
    {
        while (!IsBothHandsPinching())
        {
            yield return null;
        }
        // Wait for gesture release to prevent accidental double-triggers
        while (IsBothHandsPinching())
        {
            yield return null;
        }
    }

    private bool IsBothHandsPinching()
    {
        if (!rightHand.IsTracked || !leftHand.IsTracked)
            return false;

        bool rightPinching = rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
        bool leftPinching = leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

        return rightPinching && leftPinching;
    }



    private void CleanupRound(Dictionary<GameObject, Vector3> originalPositions, List<GameObject> ghostObjects)
    {
        // Clean up original objects and their line renderers
        foreach (var entry in originalPositions)
        {
            GameObject obj = entry.Key;
            LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                Destroy(lineRenderer);
            }
            Destroy(obj);
        }

        // Clean up ghost objects
        foreach (GameObject ghost in ghostObjects)
        {
            Destroy(ghost);
        }
        ghostObjects.Clear();

        // Clean up distance text objects
        TextMeshPro[] distanceTexts = FindObjectsOfType<TextMeshPro>();
        foreach (TextMeshPro text in distanceTexts)
        {
            if (text != distanceTextOriginal)
            {
                Destroy(text.gameObject);
            }
        }
    }

    private IEnumerator StartTask7()
    {
        totalDistance = 0f;
        totalMeasurements = 0;

        for (int iteration = 0; iteration < 10; iteration++)
        {
            Debug.Log($"Starting iteration {iteration + 1} of 10");

            List<GameObject> chosenObjects = PlaceObjectsRandomly();
            Dictionary<GameObject, Vector3> originalPositions = SaveOriginalPositions(chosenObjects);

            yield return new WaitForSeconds(3f);

            PlaceObjectsInLine(chosenObjects);

            yield return WaitForUserInput();

            List<GameObject> ghostObjects = DrawDistancesToOriginalObjects(originalPositions);

            yield return WaitForUserInput();

            CleanupRound(originalPositions, ghostObjects);

            yield return new WaitForSeconds(3f);
        }

        ShowAverageDistance();
        yield return null;
    }

    private void ShowAverageDistance()
    {
        float averageDistance = totalDistance / totalMeasurements;
        Debug.Log($"Average distance across all rounds: {averageDistance:F1} cm");

        TextMeshPro averageText = Instantiate(distanceTextOriginal);
        averageText.text = $"Average Distance: {averageDistance:F1} cm";
        averageText.transform.position = tableCenter + new Vector3(0, 0.5f, 0);
        averageText.transform.LookAt(Camera.main.transform);
        averageText.transform.Rotate(0, 180, 0);
        averageText.color = CalculateColor(averageDistance);
    }

    private GameObject ShowOriginalObject(GameObject obj, Vector3 originalPosition)
    {
        GameObject ghostObj = Instantiate(obj);

        // Get the offset between object center and pivot
        Bounds bounds = obj.GetComponent<Collider>().bounds;
        Vector3 centerOffset = bounds.center - obj.transform.position;

        // Position at original location accounting for center offset
        ghostObj.transform.position = originalPosition - centerOffset;

        // Make the ghost object non-interactive
        Destroy(ghostObj.GetComponent<Rigidbody>());
        Destroy(ghostObj.GetComponent<Collider>());

        // Make it semi-transparent green
        foreach (Renderer renderer in ghostObj.GetComponentsInChildren<Renderer>())
        {
            Material material = renderer.material;
            Color greenTint = new Color(0.4f, 1f, 0.4f, 0.5f); // Light green with 50% transparency
            material.color = greenTint;
        }

        return ghostObj;
    }

    private Color CalculateColor(float distance)
    {
        // clamp value between 0 and 5
        float c = Mathf.Clamp(distance, 0f, 50f);

        // calculate the color interpolation factor
        float t = c / 50;

        // interpolate between green and red
        return Color.Lerp(Color.green, Color.red, t);
    }

    private List<GameObject> DrawDistancesToOriginalObjects(Dictionary<GameObject, Vector3> originalPositions)
    {
        List<GameObject> ghostObjects = new List<GameObject>();

        foreach (var entry in originalPositions)
        {
            GameObject obj = entry.Key;
            Vector3 originalPosition = entry.Value;

            // Get the object's center using its bounds
            Bounds bounds = obj.GetComponent<Collider>().bounds;
            Vector3 currentPosition = bounds.center;
            Vector3 originalCenter = originalPosition + (bounds.center - obj.transform.position);

            // Create new LineRenderer component
            LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.useWorldSpace = true;
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.SetPosition(0, originalCenter);
            lineRenderer.SetPosition(1, currentPosition);


            TextMeshPro distanceText = Instantiate(distanceTextOriginal);

            // Calculate distance
            float lineLength = Vector3.Distance(originalCenter, currentPosition) * 100;

            totalDistance += lineLength;
            totalMeasurements++;

            Color gradedColor = CalculateColor(lineLength);
            lineRenderer.startColor = gradedColor;
            lineRenderer.endColor = gradedColor;

            // Position the text at the midpoint of the line
            Vector3 midpoint = (originalCenter + currentPosition) / 2;

            // Update text
            distanceText.text = lineLength.ToString("F1") + " cm";
            distanceText.transform.position = midpoint + textOffset;
            distanceText.transform.LookAt(Camera.main.transform);
            distanceText.transform.Rotate(0, 180, 0);
            distanceText.color = gradedColor;

            ghostObjects.Add(ShowOriginalObject(obj, originalPosition));

        }

        return ghostObjects;
    }

    private Dictionary<GameObject, Vector3> SaveOriginalPositions(List<GameObject> chosenObjects)
    {
        Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

        foreach (GameObject obj in chosenObjects)
        {
            if (obj != null)
            {
                originalPositions[obj] = obj.transform.position;
            }
        }

        return originalPositions;
    }

    private void PlaceObjectsInLine(List<GameObject> chosenObjects)
    {
        float totalObjects = chosenObjects.Count;
        float spacing = tableWidth / (totalObjects + 1);

        Vector3 startPosition = new Vector3(
            tableCenter.x,
            tableTopY,
            tableCenter.z - (tableWidth / 2) + spacing
        );

        for (int i = 0; i < chosenObjects.Count; i++)
        {
            if (chosenObjects[i] != null)
            {

                // Calculate the desired center position for the object
                Vector3 desiredCenter = startPosition + new Vector3(0, GetHalfHeight(chosenObjects[i]), spacing * i);

                chosenObjects[i].transform.position = desiredCenter;
                chosenObjects[i].GetComponent<Grabbable>().enabled = true;
                chosenObjects[i].GetComponent<HandGrabInteractable>().enabled = true;
                chosenObjects[i].GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    private List<GameObject> PlaceObjectsRandomly()
    {
        //int numObjectsToPlace = Random.Range(minObjects, maxObjects + 1);
        int numObjectsToPlace = 3;
        List<GameObject> selectionList = new List<GameObject>(allObjects);
        List<GameObject> chosenObjects = new List<GameObject>();

        for (int i = 0; i < numObjectsToPlace; i++)
        {
            int index = Random.Range(0, selectionList.Count);
            GameObject originalObj = selectionList[index];

            GameObject objCopy = Instantiate(originalObj);

            // Deactivate Oculus grab-related components instead of destroying them
            objCopy.GetComponent<Grabbable>().enabled = false;
            objCopy.GetComponent<HandGrabInteractable>().enabled = false;
            objCopy.GetComponent<Rigidbody>().isKinematic = true;

            objCopy.transform.position = GetRandomPositionOnTableTop(objCopy);

            chosenObjects.Add(objCopy);
            selectionList.RemoveAt(index);
        }

        return chosenObjects;
    }

    Vector3 GetRandomPositionOnTableTop(GameObject objectToPlace)
    {
        float x = Random.Range(tableCenter.x - (tableSize.x / 2) + marginX,
                              tableCenter.x + (tableSize.x / 2) - marginX);
        float z = Random.Range(tableCenter.z - (tableSize.z / 2) + marginZ,
                              tableCenter.z + (tableSize.z / 2) - marginZ);
        float y = tableTopY + GetHalfHeight(objectToPlace);

        // Get the offset between the object's center and its pivot
        Bounds bounds = objectToPlace.GetComponent<Collider>().bounds;
        Vector3 centerOffset = bounds.center - objectToPlace.transform.position;

        // Return position adjusted for center offset
        return new Vector3(x, y, z) - centerOffset;
    }

    private float GetHalfHeight(GameObject obj)
    {
        Collider objectCollider = obj.GetComponent<Collider>();
        if (objectCollider != null)
        {
            // Use the actual height from center to top
            return objectCollider.bounds.size.y / 2;
        }
        else
        {
            Debug.LogWarning("No Collider found on the object to determine half height.");
            return 0f;
        }
    }
}
