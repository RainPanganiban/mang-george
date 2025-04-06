using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ActivateGame : MonoBehaviour
{

    public GameObject player;
    public GameObject healthUI;
    public GameObject countdown;
    public GameObject weapon;
    public GameObject tutorialOverlay;


    [SerializeField]
    [Header("Enemies")]
    public GameObject currentBoss;

    public void Start()
    {
        currentBoss = GameObject.FindGameObjectWithTag("Enemy");
    }

    public void ActivatePlayerAndUI()
    {
        healthUI.SetActive(true);
        countdown.SetActive(false);

        player.GetComponent<PlayerController>().enabled = true;
        weapon.GetComponent<Weapon>().enabled = true;
        currentBoss.GetComponent<MonoBehaviour>().enabled = true;

        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            tutorialOverlay.SetActive(true);
        }
        else
        {
            tutorialOverlay.SetActive(false);
        }

    }

}
