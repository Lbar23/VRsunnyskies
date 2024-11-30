using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TutorialPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip task1;
    public VideoClip task2;
    public VideoClip task3;
    public VideoClip task4;
    public VideoClip task5;
    public VideoClip task6;
    public VideoClip task7;

    private int taskResult;

    IEnumerator Start()
    {
        // Retrieve the task result from PlayerPrefs
        taskResult = PlayerPrefs.GetInt("TaskResult", 0); // Default to 0 if not set

        // Select the video based on the task result
        switch (taskResult)
        {
            case 1:
                videoPlayer.clip = task1;
                break;
            case 2:
                videoPlayer.clip = task2;
                break;
            case 3:
                videoPlayer.clip = task3;
                break;
            case 4:
                videoPlayer.clip = task4;
                break;
            case 5:
                videoPlayer.clip = task5;
                break;
            case 6:
                videoPlayer.clip = task6;
                break;
            case 7:
                videoPlayer.clip = task7;
                break;
            default:
                Debug.LogWarning("No valid task result found. Playing default video.");
                videoPlayer.clip = task1; // Default video
                break;
        }
        
        yield return new WaitForSeconds(1f); // Wait for one second

        // Play the selected video
        videoPlayer.Play();

        // Subscribe to the loopPointReached event
        videoPlayer.loopPointReached += EndReached;
    }

    void EndReached(VideoPlayer vp)
    {
        // Load the next scene when the video ends
        SceneManager.LoadScene("Task" + taskResult);
    }
}
