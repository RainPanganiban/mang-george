using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ManananggalUpperBody : MonoBehaviour, IDamageable
{
    public int maxHealth = 50;
    private int currentHealth;
    private Animator animator;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Movement Settings")]
    public float ascendHeight = 5f;
    public float ascendSpeed = 2f;
    public float moveSpeed = 2f;
    public float moveRange = 3f;

    private bool hasAscended = false;
    private float randomOffset;

    [Header("Attack Settings")]
    public GameObject bloodProjectilePrefab;  // Prefab for falling blood drops
    public Transform bloodSpawnPoint;         // Where the blood projectiles spawn
    public float bloodRainInterval = 0.2f;    // Delay between blood drops
    public int bloodRainAmount = 5;           // Number of blood projectiles per attack

    public float attackCooldown = 5f;         // Time between attacks
    private bool canAttack = true;

    public float wingAttackRange = 3f;        // Range of the wing attack
    public int wingAttackDamage = 10;         // Damage of the wing attack
    public float knockbackForce = 5f;         // Knockback force for the player

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }

        randomOffset = Random.Range(0f, Mathf.PI * 2f);

        // Start the attack cycle
        StartCoroutine(AttackLoop());
    }

    void Update()
    {
        if (!hasAscended)
        {
            AscendAbovePlayer();
        }
        else
        {
            MoveRandomlyAbovePlayer();
        }

        FacePlayer();
    }

    void FacePlayer()
    {
        if (player == null) return;
        spriteRenderer.flipX = player.position.x > transform.position.x;
    }

    void AscendAbovePlayer()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(player.position.x, player.position.y + ascendHeight, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, ascendSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            hasAscended = true;
        }
    }

    void MoveRandomlyAbovePlayer()
    {
        if (player == null) return;

        float xOffset = Mathf.Sin(Time.time + randomOffset) * moveRange;
        Vector3 targetPosition = new Vector3(player.position.x + xOffset, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.07f);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    // Attack Loop - Triggers random attack every few seconds
    IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);
            PerformRandomAttack();
        }
    }

    // Picks a random attack
    void PerformRandomAttack()
    {
        if (!canAttack) return;

        int randomAttack = Random.Range(0, 2);  // 0 = Blood Rain, 1 = Wing Attack

        if (randomAttack == 0)
        {
            StartCoroutine(BloodRainAttack());
        }
        else
        {
            WingAttack();
        }
    }

    // Blood Rain Attack
    IEnumerator BloodRainAttack()
    {
        canAttack = false;
        Debug.Log("Manananggal is using Blood Rain!");

        // Define the spawn range relative to the player
        float spawnWidth = 6f; // Total width the projectiles will spawn across
        int projectileCount = bloodRainAmount; // Number of blood projectiles
        float spacing = spawnWidth / (projectileCount - 1); // Space between each projectile

        for (int i = 0; i < projectileCount; i++)
        {
            // Calculate spawn position (evenly spaced across X)
            float xOffset = -spawnWidth / 2 + i * spacing; // Left to right spread
            Vector3 spawnPosition = new Vector3(player.position.x + xOffset, bloodSpawnPoint.position.y, 0);

            // Spawn the projectile
            Instantiate(bloodProjectilePrefab, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(bloodRainInterval);
        }

        canAttack = true;
    }

    // Wing Attack
    void WingAttack()
    {
        canAttack = false;
        Debug.Log("Manananggal is using Wing Attack!");

        // Play animation
        animator.SetTrigger("WingAttack");

        // Check if player is within range
        if (player != null && Vector3.Distance(transform.position, player.position) <= wingAttackRange)
        {
            // Apply damage (assuming player has a TakeDamage function)
            player.GetComponent<IDamageable>()?.TakeDamage(wingAttackDamage);

            // Apply knockback
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (player.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }

        // Cooldown before next attack
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
