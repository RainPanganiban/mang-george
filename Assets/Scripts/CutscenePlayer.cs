using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutscenePlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string sceneToLoad = "Tutorial";
    public GameObject skipButton;
    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        if (skipButton != null) {
            StartCoroutine(BlinkSkipButton());
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void SkipCutscene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator BlinkSkipButton()
    {
        
        CanvasGroup canvasGroup = skipButton.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = skipButton.AddComponent<CanvasGroup>();
        }

        float duration = 1f; // fade duration

        while (true)
        {
            // Fade out
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
                yield return null;
            }
            canvasGroup.alpha = 0f;

            // Fade in
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
    }

}
