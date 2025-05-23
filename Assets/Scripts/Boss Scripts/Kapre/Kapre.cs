using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kapre : MonoBehaviour, IDamageable
{
    [SerializeField]
    [Header("Health Settings")]
    public int maxHealth = 400;
    private float currentHealth;
    public Slider slider;

    [SerializeField]
    [Header("Attack Settings")]
    public float attackInterval = 5f;
    private float attackTimer;
    public float attackCooldownTime = 2f;
    private bool canAttack = true;

    [SerializeField]
    [Header("Phase 1")]
    public GameObject smokePrefab;
    public Transform smokePoint;
    public GameObject spikePrefab;
    public int spikeCount = 5;
    public float spikeSpacing = 1.5f;
    public LayerMask groundLayer;

    /*[SerializeField]
    [Header("Phase 2")]
    public float walkSpeed = 2f;
    public float smashRange = 3f;
    public float smashWindUpTime = 1.5f;
    public float smashDamage = 10f;
    public Transform smashPoint;
    public float smashRadius = 1.5f;
    public LayerMask playerLayer;
    public float teleportDistance = 2f;*/

    [SerializeField]
    [Header("Phase 2")]
    public GameObject shockwaveLinePrefab;
    public Transform shockwaveSpawnPoint;
    public GameObject lightningPrefab;
    public GameObject lineIndicatorPrefab;
    public float stormDelayBetweenStrikes = 0.6f;
    public float indicatorDuration = 0.4f;
    public float stormYPosition = 2f;
    public float stormCallCooldownDuration = 10f;

    [SerializeField]
    [Header("Phase 3")]
    public GameObject giantKaprePrefab;

    [SerializeField]
    [Header("Phase Settings")]
    public int currentPhase = 1;
    private bool isTransitioning = false;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;
    private bool isInvincible = false;
    private bool isAttacking = false;

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

        if (!isTransitioning && !isAttacking)
        {
            HandleAttacks();
        }

        FacePlayer();
    }

    void FacePlayer()
    {
        if (player == null) return;

        bool isFacingRight = player.position.x > transform.position.x;
        spriteRenderer.flipX = player.position.x > transform.position.x;
    }

    void HandlePhases()
    {
        if (currentHealth > 300 && currentPhase != 1)
        {
            currentPhase = 1;
            animator.SetInteger("Phase", currentPhase);
        }
        else if (currentHealth <= 300 && currentHealth > 150 && currentPhase != 2)
        {
            currentPhase = 2;
            animator.SetInteger("Phase", currentPhase);
        }
        else if (currentHealth <= 150 && currentPhase != 3)
        {
            currentPhase = 3;
            animator.SetInteger("Phase", currentPhase);
        }
    }

    void HandleAttacks()
    {
        if (!canAttack) return;

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            int randomAttack;

            switch (currentPhase)
            {
                case 1:
                    randomAttack = Random.Range(0, 2);
                    animator.SetTrigger(randomAttack == 0 ? "Smoke" : "Spike");
                    StartCoroutine(AttackCooldown());
                    break;

                case 2:
                    randomAttack = Random.Range(0, 2);
                    if (randomAttack == 0)
                        animator.SetTrigger("Fireball");
                    else
                        animator.SetTrigger("Hand Smash Wave");
                    break;

                case 3:
                    Phase3Transition();
                    break;
            }
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        Debug.Log("Attack Cooldown started");
        yield return new WaitForSeconds(attackCooldownTime);
        attackTimer = attackInterval;
        canAttack = true;
        Debug.Log("Attack Cooldown ended, ready to attack again");
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        slider.value = currentHealth;

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
            Die();
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

    public void PerformSmoke()
    {
        Instantiate(smokePrefab, smokePoint.position, Quaternion.identity);
    }

    public void PerformSpikeWave()
    {
        Vector2 startPos = transform.position;
        Vector2 playerPos = player.position;
        Vector2 direction = (playerPos - startPos).normalized;

        RaycastHit2D[] groundHits = Physics2D.RaycastAll(startPos, Vector2.down, 10f);
        Vector3 spawnBasePos = transform.position;

        foreach (var hit in groundHits)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                spawnBasePos.y = hit.point.y + 0.1f;
                break;
            }
        }

        StartCoroutine(SpawnSpikesWave(spawnBasePos, direction));
    }

    IEnumerator SpawnSpikesWave(Vector3 startPos, Vector2 direction)
    {
        bool facingRight = player.position.x > transform.position.x;
        float dir = facingRight ? 1f : -1f;

        for (int i = 0; i < spikeCount; i++)
        {
            Vector3 offset = new Vector3(i * spikeSpacing * dir, 0, 0);
            Vector3 spawnPos = startPos + offset;

            // Raycast downward to find ground
            RaycastHit2D hit = Physics2D.Raycast(spawnPos + Vector3.up * 5f, Vector2.down, 10f, groundLayer);
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                Vector3 groundPos = new Vector3(spawnPos.x, hit.point.y + 0.1f, 0);
                Instantiate(spikePrefab, groundPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(0.1f); // Wave delay
        }
    }

    /*IEnumerator ApproachAndSmash()
    {
        isAttacking = true;

        while (Vector2.Distance(transform.position, player.position) > smashRange)
        {
            animator.SetBool("isWalking", true);
            FacePlayer();

            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * walkSpeed * Time.deltaTime;

            yield return null;
        }

        animator.SetBool("isWalking", false);
        animator.SetTrigger("Smash");

        yield return new WaitForSeconds(smashWindUpTime);

        GroundSmashDamage();

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        StartCoroutine(AttackCooldown());
    }

    public void GroundSmashDamage()
    {
        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        StartCoroutine(cameraShake.Shake(0.2f, 0.2f));
        Collider2D hit = Physics2D.OverlapCircle(smashPoint.position, smashRadius, playerLayer);

        if (hit != null && hit.TryGetComponent<PlayerController>(out var playerHealth))
        {
            playerHealth.TakeDamage(smashDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (smashPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(smashPoint.position, smashRadius);
        }
    }

    public void TeleportAndSmash()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 teleportPos = (Vector2)player.position - direction * teleportDistance;

        transform.position = teleportPos;
        animator.SetTrigger("Smash2");
    }*/

    public void SpawnShockwaveLine()
    {
        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        StartCoroutine(cameraShake.Shake(0.5f, 0.5f));

        GameObject wave = Instantiate(shockwaveLinePrefab, shockwaveSpawnPoint.position, Quaternion.identity);
        Vector2 dir = spriteRenderer.flipX ? Vector2.right : Vector2.left;
        wave.GetComponent<Shockwave>().SetDirection(dir);
    }

    public void StartStormCall()
    {
        StartCoroutine(StormCallRoutine());
    }

    IEnumerator StormCallRoutine()
    {
        List<float> usedPositions = new List<float>();
        int numberOfStrikes = Random.Range(6, 10);
        float minSpacing = 2.5f;

        for (int i = 0; i < numberOfStrikes; i++)
        {
            float xPosition;

            int attempts = 0;
            do
            {
                xPosition = Random.Range(Camera.main.ViewportToWorldPoint(Vector3.zero).x + 1f,
                                         Camera.main.ViewportToWorldPoint(Vector3.one).x - 1f);
                attempts++;
            }
            while (usedPositions.Exists(x => Mathf.Abs(x - xPosition) < minSpacing) && attempts < 10);

            usedPositions.Add(xPosition);

            Vector2 spawnPos = new Vector2(xPosition, stormYPosition);

            GameObject line = Instantiate(lineIndicatorPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(indicatorDuration);

            Destroy(line);

            Instantiate(lightningPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(stormDelayBetweenStrikes);
        }

        canAttack = false;
        StartCoroutine(AttackCooldown());
    }

    public void Phase3Transition()
    {
        isTransitioning = true;
        isInvincible = true;

        giantKaprePrefab.SetActive(true);
        giantKaprePrefab.transform.position = transform.position;

        gameObject.SetActive(false);
    }


}
