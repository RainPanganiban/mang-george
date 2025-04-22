using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bungisngis : MonoBehaviour, IDamageable
{
    [SerializeField][Header("Health Settings")] public int maxHealth = 250;
    private float currentHealth;
    public Slider slider;

    [SerializeField][Header("Attack Settings")] public float attackInterval = 5f;
    private float attackTimer;
    public float attackCooldownTime = 2f;
    private bool canAttack = true;
    private int lastAttackChoice = -1;

    [SerializeField]
    [Header("Phase 1")]
    public GameObject soundWavePrefab;
    public Transform spawnPoint;
    public GameObject shockwavePrefab;
    public Transform stompSpawnPoint;

    [SerializeField]
    [Header("Phase 2")]
    public float walkSpeed = 2f;
    public float smashRange = 3f;
    public float smashWindUpTime = 1.5f;
    public float smashDamage = 10f;
    public Transform smashPoint;
    public float smashRadius = 1.5f;
    public LayerMask playerLayer;
    public GameObject clubPrefab;
    public Transform clubSpawnPoint;

    [SerializeField]
    [Header("Phase 3")]
    public GameObject eyeLaserPrefab;
    public Transform eyeLaserSpawnPoint;
    public GameObject boulderPrefab;
    public Transform boulderSpawnPoint;

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

        Vector3 anchorScale = eyeLaserSpawnPoint.localScale;
        anchorScale.x = isFacingRight ? 1 : -1;
        eyeLaserSpawnPoint.localScale = anchorScale;
    }

    void HandlePhases()
    {
        if (currentHealth > 175 && currentPhase != 1)
        {
            currentPhase = 1;
            animator.SetInteger("Phase", currentPhase);
        }
        else if (currentHealth > 75 && currentHealth <= 175 && currentPhase != 2)
        {
            currentPhase = 2;
            animator.SetInteger("Phase", currentPhase);
        }
        else if (currentHealth <= 75 && currentPhase != 3)
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
            int attackChoice;
            int randomAttack;

            switch (currentPhase)
            {
                case 1:
                    randomAttack = Random.Range(0, 2);
                    animator.SetTrigger(randomAttack == 0 ? "Stomp Quake" : "Sound Waves");
                    StartCoroutine(AttackCooldown());
                    break;

                case 2:
                    randomAttack = Random.Range(0, 2);
                    if (randomAttack == 0)
                        StartCoroutine(ApproachAndSmash());
                    else
                        StartCoroutine(ClubThrowRoutine());
                    break;

                case 3:
                    randomAttack = Random.Range(0, 2);
                    animator.SetTrigger(randomAttack == 0 ? "Laser" : "Boulder");
                    StartCoroutine(AttackCooldown());
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

    public void PerformSoundWave()
    {
        Vector2 direction = (player.position - spawnPoint.position).normalized;
        GameObject wave = Instantiate(soundWavePrefab, spawnPoint.position, Quaternion.identity);
        wave.GetComponent<SoundWave>().SetDirection(direction);

        StartCoroutine(AttackCooldown());
    }

    public void PerformStompShockwave()
    {
        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        GameObject wave = Instantiate(shockwavePrefab, stompSpawnPoint.position, Quaternion.identity);
        StartCoroutine(cameraShake.Shake(0.3f, 0.3f));
        Vector2 dir = spriteRenderer.flipX ? Vector2.right : Vector2.left;
        wave.GetComponent<StompShockwave>().SetDirection(dir);
    }

    IEnumerator ApproachAndSmash()
    {
        isAttacking = true;
        animator.ResetTrigger("Club Throw");

        while (Vector2.Distance(transform.position, player.position) > smashRange)
        {
            animator.SetBool("isWalking", true);
            FacePlayer();

            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * walkSpeed * Time.deltaTime;

            yield return null;
        }

        animator.SetBool("isWalking", false);
        animator.SetTrigger("Ground Smash");

        yield return new WaitForSeconds(smashWindUpTime); // wait for windup animation

        GroundSmashDamage();

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        StartCoroutine(AttackCooldown());
    }


    public void GroundSmashDamage()
    {
        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        StartCoroutine(cameraShake.Shake(0.3f, 0.3f));
        Collider2D hit = Physics2D.OverlapCircle(smashPoint.position, smashRadius, playerLayer);

        if (hit != null && hit.TryGetComponent<PlayerController>(out var playerHealth))
        {
            playerHealth.TakeDamage(smashDamage);
        }
    }

    public void PerformClubThrow()
    {
        GameObject club = Instantiate(clubPrefab, clubSpawnPoint.position, Quaternion.identity);
        club.GetComponent<Club>().Initialize(clubSpawnPoint.position, this.transform);
    }

    IEnumerator ClubThrowRoutine()
    {
        isAttacking = true;
        animator.SetTrigger("Club Throw");

        yield return new WaitForSeconds(2.5f);

        animator.ResetTrigger("Club Throw");
        isAttacking = false;
        StartCoroutine(AttackCooldown());
    }

    void OnDrawGizmosSelected()
    {
        if (smashPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(smashPoint.position, smashRadius);
        }
    }

    public void SpawnEyeLaser()
    {
        bool topToBottom = Random.value > 0.5f;
        bool isFacingRight = spriteRenderer.flipX;

        GameObject laser = Instantiate(eyeLaserPrefab, eyeLaserSpawnPoint.position, Quaternion.identity);

        EyeLaser eyeLaser = laser.GetComponent<EyeLaser>();
        eyeLaser.faceRight = isFacingRight;
    }

    public void PerformBoulderThrow()
    {
        Instantiate(boulderPrefab, boulderSpawnPoint.position, Quaternion.identity);
    }
}
