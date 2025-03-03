using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance; // Singleton: Makes this accessible from anywhere

    public int maxHealth = 100;
    public int currentHealth;
    public float moveSpeed = 5f;
    public float dashCooldown = 2f;
    public float damageMultiplier = 1.0f;

    void Awake()
    {
        // Make sure there is only ONE instance of PlayerStats that carries across levels
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps stats when loading a new scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHealth = maxHealth; // Set starting health
    }

    public void ApplyUpgrade(string upgradeType)
    {

        Debug.Log("Applying Upgrade: " + upgradeType);

        // Applies the upgrade based on type
        switch (upgradeType)
        {
            case "Health":
                maxHealth += 20; // Increase max HP
                currentHealth = maxHealth; // Heal player
                break;
            case "Speed":
                moveSpeed += 1.5f; // Increase movement speed
                break;
            case "Dash":
                dashCooldown -= 0.5f; // Reduce dash cooldown
                break;
            case "Damage":
                damageMultiplier += 0.5f; // Increase damage output
                break;
        }

        Debug.Log("New Stats -> Health: " + maxHealth + ", Speed: " + moveSpeed + ", Dash Cooldown: " + dashCooldown + ", Damage Multiplier: " + damageMultiplier);
    }
}
