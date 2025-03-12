using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Manananggal : MonoBehaviour, IDamageable
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
    private int activeBats = 0;

    [Header("Phase Settings")]
    public int currentPhase = 1;
    private bool isTransitioning = false;

    [Header("Phase 2 Transition")]
    public GameObject upperBodyPrefab; // Prefab for the flying upper body
    public GameObject lowerBodyPrefab; // Prefab for the stationary lower body

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        attackTimer = attackInterval;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        HandlePhases();
        if (!isTransitioning)
        {
            HandleAttacks();
            FacePlayer();
        }
    }

    void FacePlayer()
    {
        if (player == null) return;
        spriteRenderer.flipX = player.position.x > transform.position.x;
    }

    void HandlePhases()
    {
        if (currentHealth > 50)
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
                StartCoroutine(StartPhase2Transition());
            }
        }
    }

    IEnumerator StartPhase2Transition()
    {
        isInvincible = true;

        animator.SetTrigger("Split"); // Play splitting animation

        yield return new WaitForSeconds(2f);

        // Disable full-body sprite
        gameObject.SetActive(false);

        // Instantiate Upper & Lower Body
        Instantiate(upperBodyPrefab, transform.position, Quaternion.identity);
        Instantiate(lowerBodyPrefab, transform.position, Quaternion.identity);
    }

    void HandleAttacks()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0 && activeBats == 0) // Ensures bats are gone before summoning new ones
        {
            Attack();
            attackTimer = attackInterval;
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Bat Summon"); // Play summon animation
        }

        StartCoroutine(SummonBatsAfterAnimation());
    }

    IEnumerator SummonBatsAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        HomingPaniki();
        animator.SetBool("isAttacking", false);
    }

    void HomingPaniki()
    {
        if (activeBats > 0) return; // Don't spawn if bats are still active

        int batCount = 3;
        float spawnOffsetY = 3f;
        float spawnOffsetX = 1f;

        if (bulletPrefab != null && firePoint != null && player != null)
        {
            for (int i = 0; i < batCount; i++)
            {
                Vector2 spawnPosition = new Vector2(
                    transform.position.x - (spriteRenderer.flipX ? -spawnOffsetX : spawnOffsetX),
                    transform.position.y + spawnOffsetY
                );

                GameObject bat = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                Bat batScript = bat.GetComponent<Bat>();
                if (batScript != null)
                {
                    batScript.Initialize(player, spawnPosition, this);
                    activeBats++;
                }
            }
        }
    }

    public void OnBatDestroyed()
    {
        activeBats--;
    }

    void Airburst()
    {
        if (bulletPrefab != null && firePoint != null)
        {

        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        slider.value = currentHealth;

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
}
