using UnityEngine;
using System.Collections;
using OculusSampleFramework;
using TMPro;
using Oculus.Platform;
using UnityEngine.SceneManagement;

public class RandomSpawnInFrontOfCamera : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRHand rightHand;

    public GameObject leftHandModel;
    public GameObject rightHandModel;

    public GameObject ball;
    public TextMeshPro distanceText;
    public TextMeshPro screenMessage;
    public TextMeshPro gameTimerNumber;
    public TextMeshPro generalPurposeText;

    private float distanceSum = 0;
    public int roundCount = 8;
    private static MeshRenderer ballRenderer;
    private static LineRenderer fingerToBallLine;
    private static Vector3 textOffset = new Vector3(0, 0.05f, 0);
    private static float pinchThreshold = 0.93f;


    public void Start()
    {

        Logger.Log("=========== Task 5: Starting ===========");
        ballRenderer = ball.GetComponent<MeshRenderer>();

        fingerToBallLine = gameObject.AddComponent<LineRenderer>();
        fingerToBallLine.startWidth = 0.007f;
        fingerToBallLine.endWidth = 0.007f;
        fingerToBallLine.material = new Material(Shader.Find("Sprites/Default"));
        fingerToBallLine.positionCount = 2;

        StartCoroutine(InitializeHandTracking());
    }



    private IEnumerator InitializeHandTracking()
    {

        yield return new WaitForSeconds(1f);

        WriteMessage("To start the task, hold both hands up in front of you.", 30, Color.green, 2);

        yield return new WaitUntil(() => leftHand.IsTracked && rightHand.IsTracked);

        // remove text 
        generalPurposeText.enabled = false;

        yield return StartCoroutine(StartTimer(3));

        // start spawning the ball as soon as hand tracking initialization is ready
        yield return StartCoroutine(StartTask());
    }

    private IEnumerator StartTimer(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            gameTimerNumber.text = "" + i;
            yield return new WaitForSeconds(1f);
        }

        gameTimerNumber.text = "";
    }

    private (OVRHand hand, GameObject handModel, Vector3 indexFingerTipPosition) GetIndexFingerTipPosition(OVRHand leftHand, OVRHand rightHand)
    {
        OVRSkeleton leftSkeleton = leftHand.GetComponent<OVRSkeleton>();
        OVRSkeleton rightSkeleton = rightHand.GetComponent<OVRSkeleton>();

        Vector3 leftPosition;
        Vector3 rightPosition;

        if (leftSkeleton == null || leftSkeleton.Bones == null)
        {
            leftPosition = Vector3.zero;
        }
        else
        {
            OVRBone leftIndexFingerTip = leftSkeleton.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip];
            leftPosition = leftIndexFingerTip.Transform.position;
        }



        if (rightSkeleton == null || rightSkeleton.Bones == null)
        {
            rightPosition = Vector3.zero;
        }
        else
        {
            OVRBone rightIndexFingerTip = rightSkeleton.Bones[(int)OVRPlugin.BoneId.Hand_IndexTip];
            rightPosition = rightIndexFingerTip.Transform.position;
        }


        float leftDistance = Vector3.Distance(leftPosition, ball.transform.position);
        float rightDistance = Vector3.Distance(rightPosition, ball.transform.position);


        // only returns the hand that is closer to the ball
        if (leftDistance < rightDistance)
        {
            return (leftHand, leftHandModel, leftPosition);
        }
        else
        {
            return (rightHand, rightHandModel, rightPosition);
        }

    }


    private IEnumerator StartTask()
    {

        for (int i = 0; i < roundCount; i++)
        {

            Logger.Log($"Task 5: Starting iteration {i + 1} of {roundCount}");

            SpawnBall();
            screenMessage.text = "Focus on the ball and its location within space.";
            yield return new WaitForSeconds(5f);

            // now disable ball and try to point to its position

            ballRenderer.enabled = false;
            screenMessage.text = "Point to where the ball was at with you index finger.\nPinch your thumb and index finger to continue.";

            yield return new WaitUntil(() => IsPinching(leftHand) || IsPinching(rightHand));

            var (hand, handModel, indexFingerTipPosition) = GetIndexFingerTipPosition(leftHand, rightHand);


            // - 1.5 from ball to ball center
            float lineLength = Vector3.Distance(indexFingerTipPosition, ball.transform.position) * 100 - 1.5f;
            // ensures that there are no negative distances
            if (lineLength < 0) lineLength = 0;

            if(lineLength > 50) {
                Logger.Log($"Task 5: Misread hand signal. Restarting iteration");
                i--;
                continue;
            }

            screenMessage.text = "See how close you were.";

            ballRenderer.enabled = true;


            DrawDistance(indexFingerTipPosition, ball.transform.position);
            GameObject clonedHand = DrawHandAtTimeOfGuess(hand, handModel);

            yield return StartCoroutine(StartTimer(5));

            // remove ball, line and text before next round
            fingerToBallLine.enabled = false;
            distanceText.enabled = false;
            ballRenderer.enabled = false;
            screenMessage.text = "Next round starting in:";
            Destroy(clonedHand);

            yield return StartCoroutine(StartTimer(2));

        }

        DrawAverage();

        yield return new WaitForSeconds(3f);

        JumpToNextTask();

    }


    private void JumpToNextTask()
    {
        if (PlayerPrefs.GetInt("TutorialMode", 0) == 1)
        {
            PlayerPrefs.SetInt("NextTutorial", 6);
            SceneManager.LoadScene("TutorialPlayer");
        }
        else
        {
            SceneManager.LoadScene("Task6");
        }
    }


    private bool IsPinching(OVRHand hand)
    {
        return hand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold;
    }

    private void SpawnBall()
    {
        // randomly choose a distance
        float distance = Random.Range(0.25f, 1.3f);

        // 0.25 and 0.75 so that the ball doesnt spawn to close to the edges of the viewport
        Vector3 randomViewportPosition = new Vector3(Random.Range(0.25f, 0.75f), Random.Range(0.3f, 0.7f), distance);
        Vector3 spawnPosition = Camera.main.ViewportToWorldPoint(randomViewportPosition);

        // copy the ball prefab
        ball.transform.position = spawnPosition;
        ballRenderer.enabled = true;

    }


    private void DrawAverage()
    {

        float average = distanceSum / roundCount;

        string message = "Your average is: " + average.ToString("F1") + " cm";

        WriteMessage(message, 26, CalculateColor(average), 2);

        Logger.Log($"Task 5: Average Distance: {average:F1} cm");

    }

    private Color CalculateColor(float distance)
    {
        // clamp value between 0 and 5
        float c = Mathf.Clamp(distance, 0f, 5f);

        // calculate the color interpolation factor
        float t = c / 5;

        // interpolate between green and red
        return Color.Lerp(Color.green, Color.red, t);
    }

    private void WriteMessage(string message, int fontsize, Color color, float distance)
    {
        Debug.Log(color.ToString());
        generalPurposeText.text = message;
        generalPurposeText.color = color;
        generalPurposeText.fontSize = fontsize;
        generalPurposeText.transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
        generalPurposeText.transform.LookAt(Camera.main.transform);
        generalPurposeText.transform.Rotate(0, 180, 0);
        generalPurposeText.enabled = true;

    }

    private void DrawDistance(Vector3 pointA, Vector3 pointB)
    {

        
        // - 1.5 from ball to ball center
        float lineLength = Vector3.Distance(pointA, pointB) * 100 - 1.5f;
        // ensures that there are no negative distances
        if (lineLength < 0) lineLength = 0;

        fingerToBallLine.enabled = true;
        distanceText.enabled = true;

        fingerToBallLine.SetPosition(0, pointA);
        fingerToBallLine.SetPosition(1, pointB);


        
        Logger.Log($"Task 7: Distance to ball: {lineLength:F1} cm");

        // add to average sum
        distanceSum += lineLength;

        // Position the text at the midpoint of the line
        Vector3 midpoint = (pointA + pointB) / 2;
        Color gradedColor = CalculateColor(lineLength);

        fingerToBallLine.startColor = gradedColor;
        fingerToBallLine.endColor = gradedColor;
        distanceText.color = gradedColor;

        distanceText.text = lineLength.ToString("F1") + " cm";  // format to 1 decimal place
        distanceText.transform.position = midpoint + textOffset;
        distanceText.transform.LookAt(Camera.main.transform);
        distanceText.transform.Rotate(0, 180, 0);
        distanceText.color = gradedColor;

    }
    private GameObject DrawHandAtTimeOfGuess(OVRHand hand, GameObject handModel)
    {

        GameObject handClone = Instantiate(handModel, hand.transform.position, hand.transform.rotation);
        handClone.transform.localScale *= 1.5f;

        return handClone;
    }


}

