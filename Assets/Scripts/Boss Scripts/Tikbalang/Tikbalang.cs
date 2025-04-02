using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tikbalang : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int maxHealth = 150;
    private float currentHealth;
    public Slider slider;

    [Header("Attack Settings")]
    public float attackInterval = 5f;
    private float attackTimer;
    public float attackCooldownTime = 2f; // Cooldown time between attacks
    private bool canAttack = true; // Controls whether the boss can attack
    private int lastAttackChoice = -1;

    [Header("Phase 1")]
    public GameObject spearPrefab;
    public GameObject rockPrefab;
    public Transform firePoint;
    public float throwForce = 12f;
    public float spearSpeed = 7f;
    public float throwHeight = 4f;

    [Header("Phase 2")]
    private bool isCharging = false;
    private Vector2 chargeDirection;
    public float chargeSpeed = 12f;
    public float chargeDuration = 1.5f;

    [Header("Phase Settings")]
    public int currentPhase = 1;
    private bool isTransitioning = false;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;
    private bool isInvincible = false;
    private Rigidbody2D rb;
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
        rb = GetComponent<Rigidbody2D>();
        audioManager = FindObjectOfType<AudioManager>();
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
        if (currentHealth > 100)
        {
            if (currentPhase != 1)
            {
                currentPhase = 1;
                animator.SetInteger("Phase", currentPhase);
                Debug.Log("Entered Phase 1");
            }
        }
        else if (currentHealth > 50)
        {
            if (currentPhase != 2)
            {
                currentPhase = 2;
                animator.SetInteger("Phase", currentPhase);
                Debug.Log("Entered Phase 2");
            }
        }
        else
        {
            if (currentPhase != 3)
            {
                currentPhase = 3;
                animator.SetInteger("Phase", currentPhase);
                Debug.Log("Entered Phase 3");
            }
        }
    }

    void HandleAttacks()
    {
        if (!canAttack) return;

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            int attackChoice;

            switch (currentPhase)
            {
                case 1: // Phase 1 Attacks
                    if (Random.value < 0.7f)
                    {
                        attackChoice = (lastAttackChoice == 0) ? 1 : 0;
                    }
                    else
                    {
                        attackChoice = lastAttackChoice;
                    }

                    lastAttackChoice = attackChoice;

                    if (attackChoice == 0)
                    {
                        animator.SetTrigger("ThrowSpear");
                    }
                    else
                    {
                        animator.SetTrigger("Stomp");
                    }
                    break;

                case 2: // Phase 2 Attacks (Placeholder)
                    animator.SetTrigger("Run Indicator"); 
                    break;

                case 3: // Phase 3 Attacks (Placeholder)
                    Debug.Log("Phase 3 Attack Placeholder");
                    animator.SetTrigger("Phase3Attack");
                    break;
            }

            attackTimer = attackInterval;
            canAttack = false;
            StartCoroutine(AttackCooldown());
        }
    }

    public void StartChargeAttack()
    {
        animator.SetTrigger("Running");
        Debug.Log("Tikbalang preparing to charge...");
    }

    public void ChargeMovement()
    {
        // This function is triggered by the "Running" animation event
        Debug.Log("Tikbalang is charging!");

        isCharging = true;
        chargeDirection = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;

        // Start moving Tikbalang
        rb.velocity = new Vector2(chargeDirection.x * chargeSpeed, rb.velocity.y);

        // Set a timer to end the charge
        Invoke(nameof(EndChargeAttack), chargeDuration);
    }

    public void EndChargeAttack()
    {
        // This function is triggered by the "Charge End" animation event or timer
        Debug.Log("Tikbalang finished charging.");

        isCharging = false;
        rb.velocity = Vector2.zero; // Stop movement
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldownTime);
        canAttack = true;
    }

    public void ThrowSpear()
    {
        Vector2 playerPosition = (Vector2)player.transform.position + new Vector2(1.2f, 0);
        float throwHeight = 5f;

        GameObject spear = Instantiate(spearPrefab, firePoint.position, Quaternion.identity);
        Spear spearScript = spear.GetComponent<Spear>();

        if (spearScript != null)
        {
            spearScript.ThrowSpear(playerPosition, throwHeight);
        }

        audioManager.PlayEnemySFX(audioManager.attackingSound1);
    }

    public void StompAttack()
    {
        float groundY = -4f;
        Vector2 spawnPosition = new Vector2(player.position.x, groundY);
        Instantiate(rockPrefab, spawnPosition, Quaternion.identity);

        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        if (cameraShake != null)
        {
            StartCoroutine(cameraShake.Shake(0.2f, 0.2f));
        }

        audioManager.PlayEnemySFX(audioManager.attackingSound2);
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
