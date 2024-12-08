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

    public int numIterations = 8;

    public OVRHand rightHand;
    public OVRHand leftHand;

    private Vector3 tableCenter;
    private Vector3 tableSize;
    private float tableWidth;
    private float tableTopY;

    private static Vector3 textOffset = new Vector3(0, 0.05f, 0);

    private float totalDistance = 0f;
    private int totalMeasurements = 0;

    private List<Bounds> placedObjectBounds = new List<Bounds>();

    void Start()
    {
        Logger.Log("=========== Task 7: Starting ===========");
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
    }

    private IEnumerator WaitForUserInput()
    {
        // Wait for the user to press the button
        while (!OVRInput.Get(OVRInput.Button.One))
        {
            yield return null; // Wait until the next frame
        }

        // Wait for the user to release the button
        while (OVRInput.Get(OVRInput.Button.One))
        {
            yield return null; // Prevent double-trigger
        }

        Debug.Log("Button One Pressed!");
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

        for (int iteration = 0; iteration < numIterations; iteration++)
        {
            Logger.Log($"Task 7: Starting iteration {iteration + 1} of {numIterations}");

            List<GameObject> chosenObjects = PlaceObjectsRandomly();
            Dictionary<GameObject, Vector3> originalPositions = SaveOriginalPositions(chosenObjects);

            yield return new WaitForSeconds(3f);

            PlaceObjectsInLine(chosenObjects);

            yield return WaitForUserInput();
            //yield return new WaitForSeconds(1f);

            List<GameObject> ghostObjects = DrawDistancesToOriginalObjects(originalPositions);

            yield return WaitForUserInput();
            //yield return new WaitForSeconds(5f);

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
        averageText.transform.position = tableCenter + new Vector3(0, 1f, 0);
        averageText.transform.LookAt(Camera.main.transform);
        averageText.transform.Rotate(0, 180, 0);
        averageText.color = CalculateColor(averageDistance);

        Logger.Log($"Task 7: Average Distance: {averageDistance:F1} cm");
    }

    private GameObject ShowOriginalObject(GameObject obj, Vector3 originalPosition)
    {
        GameObject ghostObj = Instantiate(obj);

        // Simply set the position and rotation to match the original
        ghostObj.transform.position = originalPosition;
        ghostObj.transform.rotation = obj.transform.rotation;

        // Make the ghost object non-interactive
        Destroy(ghostObj.GetComponent<Rigidbody>());
        Destroy(ghostObj.GetComponent<Collider>());

        // Make it semi-transparent green
        foreach (Renderer renderer in ghostObj.GetComponentsInChildren<Renderer>())
        {
            Material material = renderer.material;
            Color greenTint = new Color(0.4f, 1f, 0.4f, 0.5f);
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
            Vector3 currentCenter = bounds.center;
            Vector3 originalCenter = originalPosition + (bounds.center - obj.transform.position);

            // Create new LineRenderer component
            LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.useWorldSpace = true;
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.SetPosition(0, originalCenter);
            lineRenderer.SetPosition(1, currentCenter);


            TextMeshPro distanceText = Instantiate(distanceTextOriginal);

            // Calculate distance
            float lineLength = Vector3.Distance(originalCenter, currentCenter) * 100;
            
            string objectName = obj.name.Replace("(Clone)", "").Trim();
            Logger.Log($"Task 7: Distance for {objectName}: {lineLength:F1} cm");

            totalDistance += lineLength;
            totalMeasurements++;

            Color gradedColor = CalculateColor(lineLength);
            lineRenderer.startColor = gradedColor;
            lineRenderer.endColor = gradedColor;

            // Position the text at the midpoint of the line
            Vector3 midpoint = (originalCenter + currentCenter) / 2;

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
        placedObjectBounds.Clear();  // Clear the bounds list before placing new objects
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

    private Vector3 GetRandomPositionOnTableTop(GameObject objectToPlace)
    {
        Bounds bounds = objectToPlace.GetComponent<Collider>().bounds;
        float objectWidth = bounds.size.x;
        float objectDepth = bounds.size.z;

        // Calculate usable table area (60% of total table size, leaving 20% margin on each side)
        float usableTableWidth = tableSize.x * 0.6f;
        float usableTableDepth = tableSize.z * 0.6f;

        while (true)
        {
            float x = Random.Range(
                tableCenter.x - (usableTableWidth / 2) + (objectWidth / 2),
                tableCenter.x + (usableTableWidth / 2) - (objectWidth / 2)
            );
            float z = Random.Range(
                tableCenter.z - (usableTableDepth / 2) + (objectDepth / 2),
                tableCenter.z + (usableTableDepth / 2) - (objectDepth / 2)
            );
            float y = tableTopY + GetHalfHeight(objectToPlace);

            Vector3 proposedPosition = new Vector3(x, y, z);

            // Create bounds for the proposed position
            Bounds proposedBounds = new Bounds(proposedPosition, bounds.size);

            // Check if this position overlaps with any existing objects
            bool overlaps = false;
            foreach (Bounds existingBounds in placedObjectBounds)
            {
                if (proposedBounds.Intersects(existingBounds))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                placedObjectBounds.Add(proposedBounds);
                return proposedPosition;
            }
        }
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
