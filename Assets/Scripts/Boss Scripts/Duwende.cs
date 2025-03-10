using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public Slider slider;

    [Header("Attack Settings")]
    public float attackInterval = 1.5f;
    private float attackTimer;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 5f;

    [Header("Phase Settings")]
    public int currentPhase = 1;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;

    [Header("Teleport Settings")]
    public float teleportInterval = 3f;
    public float minX = -7f, maxX = 7f, centerX = 0f;
    public float groundY = -4.227f;
    private bool isTeleporting = false;

    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        attackTimer = attackInterval;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Initialize the Animator
        originalColor = spriteRenderer.color;

        StartCoroutine(TeleportRoutine());
    }

    void Update()
    {
        HandlePhases();
        HandleAttacks();
        FacePlayer();
    }

    void FacePlayer()
    {
        if (player == null) return;
        spriteRenderer.flipX = player.position.x > transform.position.x;
    }

    IEnumerator TeleportRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(teleportInterval);
            Teleport();
        }
    }

    void Teleport()
    {
        if (isTeleporting) return;
        isTeleporting = true;

        float[] possiblePositions = { minX, centerX, maxX };
        float newX = possiblePositions[Random.Range(0, possiblePositions.Length)];

        transform.position = new Vector3(newX, groundY, 0);
        isTeleporting = false;
    }

    void HandlePhases()
    {
        if (currentHealth > 50) // Phase 1 (HP 100 to 50)
        {
            if (currentPhase != 1)
            {
                currentPhase = 1;
                animator.SetInteger("Phase", 1);
            }
        }
        else // Phase 2 (HP 50 and below)
        {
            if (currentPhase != 2)
            {
                currentPhase = 2;
                animator.SetInteger("Phase", 2);
                animator.Play("2nd_Attack");
            }
        }
    }

    void HandleAttacks()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = attackInterval;
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", true); // Set attack animation to true
        }

        // Start attack animation and delay shooting bullets to sync
        StartCoroutine(PerformAttackAfterDelay(0.3f));
    }

    IEnumerator PerformAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        switch (currentPhase)
        {
            case 1:
                HomingShot();
                break;
            case 2:
                SpreadShot();
                break;
        }

        yield return new WaitForSeconds(0.2f); //  Small delay before stopping animation
        animator.SetBool("isAttacking", false); // Stop attack animation
    }

    void HomingShot()
    {
        if (bulletPrefab != null && firePoint != null && player != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            Vector2 direction = (player.position - firePoint.position).normalized;
            bulletRb.velocity = direction * bulletSpeed;

            Destroy(bullet, 5f);
        }
    }

    void SpreadShot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Vector3 spawnPosition = player.position + new Vector3(0, 10f, 0);
            float[] angles = { -15f, 0f, 15f };

            foreach (float angle in angles)
            {
                GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

                Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.down;
                bulletRb.velocity = direction * bulletSpeed;

                Destroy(bullet, 5f);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        slider.value = currentHealth;

        StartCoroutine(Flashred());

        if (currentHealth <= 0)
        {
            Die();
        }

       
    }

    IEnumerator Flashred()
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
}
