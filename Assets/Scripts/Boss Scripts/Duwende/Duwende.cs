using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour , IDamageable
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    private float currentHealth;
    public Slider slider;

    [Header("Attack Settings")]
    public float attackInterval = 1.5f;
    private float attackTimer;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 5f;

    [Header("Phase Settings")]
    public int currentPhase = 1;
    public GameObject tutorialOverlay;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;
    private AudioManager audioManager;

    [Header("Teleport Settings")]
    public float teleportInterval = 3f;
    public float minX = -7f, maxX = 7f, centerX = 0f;
    public float groundY = -4.227f;
    private bool isTeleporting = false;

    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = 50;
        slider.value = currentHealth;
        attackTimer = attackInterval;
       
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
        originalColor = spriteRenderer.color;

        StartCoroutine(TeleportRoutine());
        audioManager.PlayEnemySFX(audioManager.intro);
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
        if (currentHealth > 25)
        {
            if (currentPhase != 1)
            {
                currentPhase = 1;
                animator.SetInteger("Phase", 1);
            }
        }
        else
        {
            if (currentPhase != 2)
            {
                currentPhase = 2;
                animator.SetInteger("Phase", 2);
                animator.Play("2nd_Attack");
                audioManager.PlayEnemySFX(audioManager.transition);
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
            animator.SetBool("isAttacking", true);
        }


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

        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isAttacking", false);
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

    public void TakeDamage(float damage)
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
        audioManager.PlayEnemySFX(audioManager.deathEnemy);
        GetComponent<Enemy>().enabled = false;
        tutorialOverlay.SetActive(false);

        StartCoroutine(HandleDeathSequence());

    }

    IEnumerator HandleDeathSequence()
    {
        
        yield return new WaitForSeconds(4f);
        
        UpgradeManager upgradeManager = FindAnyObjectByType<UpgradeManager>();
        if (upgradeManager != null)
        {
            upgradeManager.ShowUpgradeOptions();
        }

        Destroy(gameObject);
    }

}
