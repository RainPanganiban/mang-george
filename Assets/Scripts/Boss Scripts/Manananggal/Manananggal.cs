using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Manananggal : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private float currentHealth;
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
    public GameObject upperBodyObject;
    public GameObject lowerBodyObject;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;
    private bool isInvincible = false;
    private AudioManager audioManager;

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
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.PlayEnemySFX(audioManager.intro);


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
        audioManager.PlayEnemySFX(audioManager.transition);

        yield return new WaitForSeconds(2f);

        // Disable full-body sprite
        gameObject.SetActive(false);


        // Activate the lower body (if it's already in the scene)
        if (upperBodyObject != null)
        {
            upperBodyObject.SetActive(true);
            upperBodyObject.transform.position = transform.position;
        }
        if (lowerBodyObject != null) 
        {
            lowerBodyObject.SetActive(true);
            lowerBodyObject.transform.position = transform.position;
        }
    }


    void HandleAttacks()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0 && activeBats == 0)
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
        audioManager.PlayEnemySFX(audioManager.attackingSound1);
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

    public void TakeDamage(float damage)
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