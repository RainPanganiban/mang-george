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
    private Collider2D tikbalangCollider;

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
        tikbalangCollider = GetComponent<Collider2D>();
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
                        animator.SetTrigger("Run Indicator");
                    }
                    else
                    {
                        animator.SetTrigger("Jump");
                    }
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
        animator.SetBool("isCharging", true);
        Debug.Log("Tikbalang preparing to charge...");
    }

    public void ChargeMovement()
    {
        Debug.Log("Tikbalang is charging!");

        // Unfreeze movement but keep rotation frozen
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        tikbalangCollider.isTrigger = true;

        isCharging = true;

        // Get player direction

        Vector2 playerDirection = (player.position - transform.position).normalized;
        chargeDirection = new Vector2(Mathf.Sign(playerDirection.x), 0); // Only use horizontal direction

        // Update sprite direction based on charge direction
        spriteRenderer.flipX = chargeDirection.x > 0;

        // Set charge velocity
        rb.velocity = new Vector2(chargeDirection.x * chargeSpeed, rb.velocity.y);

        // Start checking for the screen edge
        StartCoroutine(CheckForEdge());
    }

    private IEnumerator CheckForEdge()
    {
        while (isCharging)
        {
            // Get screen edges in world coordinates
            float leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).x + 1f;  // Buffer to prevent sticking
            float rightEdge = Camera.main.ViewportToWorldPoint(Vector3.one).x - 1f;

            // Stop charging if it reaches an edge
            if ((chargeDirection.x > 0 && transform.position.x >= rightEdge) ||
                (chargeDirection.x < 0 && transform.position.x <= leftEdge))
            {
                StopCharge(); // Stop movement

                // Reset attack timer and enable attacks again
                attackTimer = 1.5f; // Shorter timer for quicker next attack
                canAttack = true;   // Enable attacks again

                yield break;  // Exit the coroutine
            }

            yield return null; // Wait for the next frame
        }
    }

    void StopCharge()
    {
        Debug.Log("Tikbalang reached the edge and stopped!");

        isCharging = false;
        rb.velocity = Vector2.zero; // Stop movement
        animator.SetBool("isCharging", false);
        // Face the player
        spriteRenderer.flipX = player.position.x > transform.position.x;
    }

    public void JumpAttack()
    {
        Debug.Log("Tikbalang is performing a Jump Attack!");

        isCharging = false; // Stop charging if already moving
        rb.velocity = Vector2.zero; // Stop movement before jumping

        // Unfreeze Y-axis to allow jumping
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Apply upward force for jumping
        rb.velocity = new Vector2(0, 20f); // Adjust jump height as needed

        // Wait 1 second in mid-air, then land on the player
        StartCoroutine(LandOnPlayer());
    }

    IEnumerator LandOnPlayer()
    {
        yield return new WaitForSeconds(1f); // Stay off-screen for suspense

        if (player == null) yield break;

        // Target player’s position
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);

        // Land with force
        rb.velocity = new Vector2(0, -20f); // Fast downward movement

        yield return new WaitForSeconds(0.5f); // Small delay before shockwave

        // Spawn shockwave attack
        SpawnShockwave();
    }

    public void SpawnShockwave()
    {
        Debug.Log("Tikbalang is sending out a shockwave!");

        float rockSpeed = 5f; // Adjust speed as needed
        int numRocks = 5; // How many rocks in each direction
        float spacing = 0.5f; // Spacing between each rock

        for (int i = 0; i < numRocks; i++)
        {
            // Spawn leftward rocks
            GameObject leftRock = Instantiate(rockPrefab, transform.position, Quaternion.identity);
            leftRock.GetComponent<Rigidbody2D>().velocity = Vector2.left * rockSpeed;

            // Spawn rightward rocks
            GameObject rightRock = Instantiate(rockPrefab, transform.position, Quaternion.identity);
            rightRock.GetComponent<Rigidbody2D>().velocity = Vector2.right * rockSpeed;

            // Adjust rock position so they spread out
            leftRock.transform.position += new Vector3(-i * spacing, 0, 0);
            rightRock.transform.position += new Vector3(i * spacing, 0, 0);
        }
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
