using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGame : MonoBehaviour
{

    public GameObject player;
    public GameObject healthUI;
    public GameObject countdown;
    public GameObject weapon;

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
    }

}
