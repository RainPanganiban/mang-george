using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneEnding : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextScene = "Main Menu";
    void Start()
    {
        videoPlayer.Play();
        videoPlayer.loopPointReached += EndReached;
    }

    void EndReached(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextScene);
    }
}
