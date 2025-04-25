using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int maxHealth = 100;
    public int currentHealth;
    public float moveSpeed = 5f;
    public float dashCooldown = 2f;
    public float damageMultiplier = 1.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void ApplyUpgrade(string upgradeType)
    {
        Debug.Log("Applying Upgrade: " + upgradeType);

        switch (upgradeType)
        {
            case "Health":
                maxHealth += 100;
                currentHealth = maxHealth; // Heal the player
                break;
            case "Speed":
                moveSpeed += 1.5f; // Increase movement speed
                break;
            case "Dash":
                dashCooldown -= 0.5f; // Decrease dash cooldown
                break;
            case "Damage":
                damageMultiplier += 0.5f; // Increase damage output
                break;
        }

        // Output the new stats to the console (for debugging purposes)
        Debug.Log("New Stats -> Health: " + maxHealth + ", Speed: " + moveSpeed + ", Dash Cooldown: " + dashCooldown + ", Damage Multiplier: " + damageMultiplier);
    }
}
