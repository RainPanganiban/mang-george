using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    

    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("CUTSCENE");
    
    }


    public void QuitGame()
    {
        Debug.Log("Application is closed");
        Application.Quit();
    }

}
