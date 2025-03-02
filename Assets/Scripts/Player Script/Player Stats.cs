using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance; // Singleton: Makes this accessible from anywhere

    public int maxHealth = 100;
    public int currentHealth;
    public float moveSpeed = 5f;
    public float dashCooldown = 2f;
    public float damage = 10f;

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
        // Applies the upgrade based on type
        switch (upgradeType)
        {
            case "Health":
                maxHealth += 20; // Increase max HP
                currentHealth += 20; // Heal player
                break;
            case "Speed":
                moveSpeed += 1.5f; // Increase movement speed
                break;
            case "Dash":
                dashCooldown -= 0.5f; // Reduce dash cooldown
                break;
            case "Damage":
                damage += 5; // Increase damage output
                break;
        }
    }
}
