using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tikbalang : MonoBehaviour, IDamageable
{
    [SerializeField]
    [Header("Health Settings")]
    public int maxHealth = 150;
    private float currentHealth;
    public Slider slider;

    [SerializeField]
    [Header("Attack Settings")]
    public float attackInterval = 5f;
    private float attackTimer;
    public float attackCooldownTime = 2f;
    private bool canAttack = true;
    private int lastAttackChoice = -1;

    [SerializeField]
    [Header("Phase 1")]
    public GameObject spearPrefab;
    public GameObject rockPrefab;
    public Transform firePoint;
    public float throwForce = 12f;
    public float spearSpeed = 7f;
    public float throwHeight = 4f;

    [SerializeField]
    [Header("Phase 2")]
    private bool isCharging = false;
    private bool isJumping = false;
    private bool isFalling;
    private Vector2 chargeDirection;
    public float chargeSpeed = 12f;
    public float chargeDuration = 1.5f;
    public BoxCollider2D headCollider;

    [SerializeField]
    [Header("Phase 3")]
    public GameObject lightningPrefab;
    public GameObject lineIndicatorPrefab;
    public float stormDelayBetweenStrikes = 0.6f;
    public float indicatorDuration = 0.4f;
    public float stormYPosition = 2f;
    public float stormCallCooldownDuration = 10f;
    private float nextStormCallTime = 0f;

    [SerializeField]
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

        audioManager.PlayEnemySFX(audioManager.intro);
    }

    void Update()
    {
        HandlePhases();
        if (!isTransitioning)
        {
            HandleAttacks();
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
                case 1:
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

                case 2:                   
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

                case 3:
                    if (Random.value < 0.7f)
                    {
                        attackChoice = 0;
                    }
                    else if (Random.value < 0.7f)
                    {
                        attackChoice = 1;
                    }
                    else
                    {
                        attackChoice = 2;
                    }

                    if (attackChoice == 2 && Time.time < nextStormCallTime)
                    {
                        attackChoice = (Random.value < 0.5f) ? 0 : 1;
                    }

                    lastAttackChoice = attackChoice;

                    if (attackChoice == 0)
                    {
                        animator.SetBool("isStormCalling", true);
                    }
                    else if (attackChoice == 1)
                    {

                        animator.SetTrigger("Run Indicator");
                    }
                    else
                    {
                        animator.SetTrigger("Stomp");
                    }
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
        audioManager.PlayEnemySFX(audioManager.attackingSound3);
    }

    public void ChargeMovement()
    {
        Debug.Log("Tikbalang is charging!");

        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        tikbalangCollider.isTrigger = true;
        headCollider.enabled = false;
        isCharging = true;
        gameObject.layer = LayerMask.NameToLayer("Default");

        chargeDirection = spriteRenderer.flipX ? Vector2.right : Vector2.left;

        rb.velocity = new Vector2(chargeDirection.x * chargeSpeed, 0);

        StartCoroutine(CheckForEdge());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCharging && other.CompareTag("Player"))
        {
            PlayerController playerDamageable = other.GetComponent<PlayerController>();
            if (playerDamageable != null)
            {
                playerDamageable.TakeDamage(20);
            }
        }
    }

    private IEnumerator CheckForEdge()
    {
        while (isCharging)
        {

            float leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).x + 3f;
            float rightEdge = Camera.main.ViewportToWorldPoint(Vector3.one).x - 3f;


            if ((chargeDirection.x > 0 && transform.position.x >= rightEdge) ||
                (chargeDirection.x < 0 && transform.position.x <= leftEdge))
            {
                StopCharge();


                attackTimer = 1.5f;
                canAttack = true;

                yield break;
            }

            yield return null;
        }
    }

    void StopCharge()
    {
        isCharging = false;
        rb.velocity = Vector2.zero; // Stop movement

        // Restore Rigidbody to normal mode
        tikbalangCollider.isTrigger = false;
        headCollider.enabled = true;
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        // Unfreeze movement but keep rotation frozen
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Stop Running Animation & Reset Charge Attack
        animator.SetBool("isCharging", false);
        animator.ResetTrigger("Run Indicator");

        // Face the player
        spriteRenderer.flipX = player.position.x > transform.position.x;

        // Allow a delay before attacking again
        StartCoroutine(AttackCooldown());
    }

    public void JumpAttack()
    {
        if (isJumping) return;

        isJumping = true;
        canAttack = false;

        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        rb.velocity = new Vector2(0, 20f);

        StartCoroutine(LandOnPlayer());
    }

    IEnumerator LandOnPlayer()
    {
        yield return new WaitForSeconds(1f);

        if (player == null) yield break;

        transform.position = new Vector2(player.position.x, transform.position.y);
        isFalling = true;
        canAttack = true;
        rb.velocity = new Vector2(0, -20f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Ground") && isJumping)
        {
            isJumping = false;
            SpawnShockwave();
            FacePlayer();
            CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
            if (cameraShake != null)
            {
                StartCoroutine(cameraShake.Shake(0.3f, 0.3f));
            }
            audioManager.PlayEnemySFX(audioManager.attackingSound5);
        }

        if (collision.gameObject.CompareTag("Player") && isFalling)
        {
            PlayerController playerDamageable = collision.gameObject.GetComponent<PlayerController>();
            if (playerDamageable != null)
            {
                playerDamageable.TakeDamage(10);
            }
            isJumping = false;
            isFalling = false;
        }
    }
    public void SpawnShockwave()
    {
        float rockSpeed = 0.5f;
        int numRocks = 5;
        float spacing = 1f;
        float fixedY = -3.5f;

        for (int i = 0; i < numRocks; i++)
        {
            Vector3 basePosition = new Vector3(transform.position.x, fixedY, 0);

            GameObject leftRock = Instantiate(rockPrefab, basePosition, Quaternion.identity);
            leftRock.GetComponent<Rigidbody2D>().velocity = Vector2.left * rockSpeed;
            leftRock.transform.position += new Vector3(-i * spacing, 0, 0);

            GameObject rightRock = Instantiate(rockPrefab, basePosition, Quaternion.identity);
            rightRock.GetComponent<Rigidbody2D>().velocity = Vector2.right * rockSpeed;
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
        audioManager.PlayEnemySFX(audioManager.attackingSound6);
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
        animator.ResetTrigger("Stomp");
    }

    public void StartStormCall()
    {
        StartCoroutine(StormCallRoutine());
    }

    IEnumerator StormCallRoutine()
    {
        for (int i = 0; i < 4; i++)
        {
            // Choose a random x position within the screen
            float xPosition = Random.Range(Camera.main.ViewportToWorldPoint(Vector3.zero).x + 1f,
                                           Camera.main.ViewportToWorldPoint(Vector3.one).x - 1f);

            Vector2 spawnPos = new Vector2(xPosition, stormYPosition);

            // Spawn line indicator
            GameObject line = Instantiate(lineIndicatorPrefab, spawnPos, Quaternion.identity);

            // Wait for the duration of the indicator
            yield return new WaitForSeconds(indicatorDuration);

            // Destroy the line just before the lightning strike
            Destroy(line);

            // Spawn the lightning bolt
            Instantiate(lightningPrefab, spawnPos, Quaternion.identity);

            // Wait before the next strike
            yield return new WaitForSeconds(stormDelayBetweenStrikes);
        }

        // Reset attack cooldown
        animator.SetBool("isStormCalling", false);
        canAttack = false;
        StartCoroutine(AttackCooldown());
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
        audioManager.PlayEnemySFX(audioManager.deathEnemy);
    }
}
