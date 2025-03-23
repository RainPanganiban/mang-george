using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class ManananggalUpperBody : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int currentHealth;
    private Animator animator;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("UI Elements")]
    public Slider bossHealthSlider; // Boss Health Bar

    [Header("Movement Settings")]
    public float ascendHeight = 5f;
    public float ascendSpeed = 2f;
    public float moveSpeed = 2f;
    public float moveRange = 3f;
    public bool canMove = true;

    private bool hasAscended = false;
    private float randomOffset;

    [Header("Attack Settings")]
    public GameObject bloodProjectilePrefab;
    public Transform bloodSpawnPoint;
    public float bloodRainInterval = 0.2f;
    public int bloodRainAmount = 5;
    public float attackCooldown = 5f;
    private bool canAttack = true;

    [Header("Wing Attack Settings")]
    public GameObject windPrefab;
    public Transform windSpawnPoint;

    public float wingAttackRange = 3f;
    public int wingAttackDamage = 10;
    public float knockbackForce = 5f;


    void Start()
    {
        currentHealth = 50;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        randomOffset = Random.Range(0f, Mathf.PI * 2f);

        // Initialize health bar
        if (bossHealthSlider != null)
        {
            bossHealthSlider.maxValue = maxHealth;
            bossHealthSlider.value = currentHealth;
        }

        StartCoroutine(AttackLoop());
    }

    void Update()
    {
        if (!canMove) return;

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
        Camera cam = Camera.main;
        if (cam == null) return;

        // Get the screen's center position in world coordinates
        Vector3 screenCenter = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.7f, 0));
        screenCenter.z = 0;

        // Movement oscillation
        float xOffset = Mathf.Sin(Time.time + randomOffset) * moveRange;
        Vector3 targetPosition = new Vector3(screenCenter.x + xOffset, screenCenter.y, transform.position.z);

        // Get camera bounds
        float halfWidth = cam.orthographicSize * cam.aspect; // Half of the screen width
        float minX = cam.transform.position.x - halfWidth + 1f; // Add some padding
        float maxX = cam.transform.position.x + halfWidth - 1f;

        // Clamp position to stay within screen bounds
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Update health bar
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = currentHealth;
        }

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
        FindAnyObjectByType<UpgradeManager>().ShowUpgradeOptions();
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);
            PerformRandomAttack();
        }
    }

    void PerformRandomAttack()
    {
        if (!canAttack) return;

        int randomAttack = Random.Range(0, 2);

        if (randomAttack == 0)
        {
            StartCoroutine(BloodRainAttack());
        }
        else
        {
            StartCoroutine(WingAttack());
        }
    }

    IEnumerator BloodRainAttack()
    {
        canAttack = false;

        float spawnWidth = 15f;
        int projectileCount = bloodRainAmount;
        float spacing = spawnWidth / (projectileCount - 1);

        for (int i = 0; i < projectileCount; i++)
        {
            float xOffset = -spawnWidth / 2 + i * spacing;
            Vector3 spawnPosition = new Vector3(player.position.x + xOffset, bloodSpawnPoint.position.y, 0);
            Instantiate(bloodProjectilePrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(bloodRainInterval);
        }

        canAttack = true;
    }

    IEnumerator WingAttack()
    {
      
        canAttack = false;
        canMove = false; // Stop movement during attack

        animator.SetTrigger("WingAttack");

        // Wait for half of the animation before spawning the wind attack
        yield return new WaitForSeconds(0.1f);

        if (player == null)
        {
            canMove = true;
            canAttack = true;
            yield break;
        }

        // Get the player's position at the moment of attack
        Vector3 targetPosition = player.position;

        // Spawn the Wind Gust
        GameObject wind = Instantiate(windPrefab, windSpawnPoint.position, Quaternion.identity);

        // Initialize wind movement with an accurate direction
        WindProjectile windScript = wind.GetComponent<WindProjectile>();
        if (windScript != null)
        {
            windScript.Initialize(targetPosition);
        }

        // Wait for the rest of the animation to complete before allowing movement
        yield return new WaitForSeconds(0.2f);

        canMove = true; // Allow movement again

        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
