using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverUI;
    private Animator animator;

    public void ShowGameOver()
    {
        StartCoroutine(DelayShowGameOver());
    }

    IEnumerator DelayShowGameOver()
    {
        yield return new WaitForSeconds(2f);

        gameOverUI.SetActive(true);
        animator.Play("Game Over");
        Time.timeScale = 0f;

    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 1");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void RestartFromTutorial()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Tutorial");
    }
}
