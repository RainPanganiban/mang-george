using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Bungisngis : MonoBehaviour, IDamageable
{
    [SerializeField]
    [Header("Health Settings")]
    public int maxHealth = 250;
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
    public GameObject soundWavePrefab;
    public Transform spawnPoint;
    public GameObject shockwavePrefab;
    public Transform stompSpawnPoint;

    [SerializeField]
    [Header("Phase 2 - Ground Smash")]
    public float walkSpeed = 2f;
    public float smashRange = 3f;
    public float smashWindUpTime = 1f;
    public float smashDamage = 10f;
    public Transform smashPoint;
    public float smashRadius = 1.5f;
    public LayerMask playerLayer;
    private Rigidbody2D rb;
    public GameObject clubPrefab;
    public Transform clubSpawnPoint;

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
        rb = GetComponent<Rigidbody2D>();
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
        spriteRenderer.flipX = player.position.x > transform.position.x;
    }

    void HandlePhases()
    {
        if (currentHealth > 175)
        {
            if (currentPhase != 1)
            {
                currentPhase = 1;
                animator.SetInteger("Phase", currentPhase);
                Debug.Log("Entered Phase 1");
            }
        }
        else if (currentHealth > 75)
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
                    int randomAttack;
                    randomAttack = Random.Range(0, 2);
                    if (randomAttack == 0)
                    {
                        animator.SetTrigger("Stomp Quake");
                    }
                    else
                    {
                        animator.SetTrigger("Sound Waves");
                    }
                    break;

                case 2:
                    randomAttack = Random.Range(0, 2);
                    if (randomAttack == 0)
                    {
                        animator.SetBool("isWalking", true);
                        StartCoroutine(ApproachAndSmash());
                    }
                    else
                    {
                        StartCoroutine(ClubThrowRoutine());
                    }
                    break;

                case 3:
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

                    }
                    else
                    {

                    }
                    break;
            }

            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldownTime);
        attackTimer = attackInterval;
        canAttack = true;
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

    public void PerformSoundWave()
    {
        Vector2 direction = (player.position - spawnPoint.position).normalized;
        GameObject wave = Instantiate(soundWavePrefab, spawnPoint.position, Quaternion.identity);
        wave.GetComponent<SoundWave>().SetDirection(direction);
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

        yield return new WaitForSeconds(smashWindUpTime);

        // Reset via animation event instead ideally
        isAttacking = false;

        StartCoroutine(AttackCooldown());
    }

    public void GroundSmashDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(smashPoint.position, smashRadius, playerLayer);

        if (hit != null)
        {
            if (hit.TryGetComponent<PlayerController>(out var playerHealth))
            {
                playerHealth.TakeDamage(smashDamage);
            }
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

    public void PerformClubThrow()
    {
        GameObject club = Instantiate(clubPrefab, clubSpawnPoint.position, Quaternion.identity);
        club.GetComponent<Club>().Initialize(clubSpawnPoint.position, this.transform);
    }

    IEnumerator ClubThrowRoutine()
    {
        isAttacking = true;

        animator.SetTrigger("Club Throw");
        yield return new WaitForSeconds(0.5f); // Sync with animation

        PerformClubThrow();

        // Reset AFTER throw animation — use animation event ideally
        isAttacking = false;

        StartCoroutine(AttackCooldown());
    }
}
