using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public static PlayerStats Instance;

    [Header("Player Stats")]

    public int maxHealth = 100;
    public float moveSpeed = 5f;
    public float dashCooldown = 1.5f;
    public int damage = 10;

    void Awake()
    {
    
        if (Instance == null) 
        {

            Instance = this;
            DontDestroyOnLoad(gameObject);

        } else
        {

            Destroy(gameObject);

        }

    }

    public void ApplyUpgrade (string upgradeType)
    {

        switch (upgradeType) 
        {

            case "Health":
                maxHealth += 20;
                break;
            case "Speed":
                moveSpeed += 1f;
                break;
            case "Dash":
                dashCooldown -= 0.5f;
                break;
            case "Damage":
                damage += 5;
                break;
        
        }

    }


}
