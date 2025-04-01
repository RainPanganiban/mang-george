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

    [Header("Phase 1")]
    public GameObject spearPrefab;
    public GameObject rockPrefab;
    public Transform firePoint;
    public float throwForce = 12f;
    public float spearSpeed = 7f;
    public float throwHeight = 4f;

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
        if (currentHealth > 50)
        {
            if (currentPhase != 1)
            {
                currentPhase = 1;
                animator.SetInteger("Phase", currentPhase);
            }
        }
        else
        {
            if (currentPhase != 2)
            {
                currentPhase = 2;
                animator.SetInteger("Phase", currentPhase);
            }
        }
    }

    void HandleAttacks()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            int attackChoice = Random.Range(0, 2); // Randomly choose between the two attacks

            if (attackChoice == 0)
            {
                animator.SetTrigger("ThrowSpear");
            }
            else
            {
                animator.SetTrigger("Stomp");
            }

            attackTimer = attackInterval;
        }
    }

    // This function is triggered via an animation event
    public void ThrowSpear()
    {
        Vector2 playerPosition = (Vector2)player.transform.position + new Vector2(1.2f, 0);
        float throwHeight = 5f; // Adjust for arc

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
        float groundY = -4f; // Set this to the fixed Y position of the ground
        Vector2 spawnPosition = new Vector2(player.position.x, groundY);
        Instantiate(rockPrefab, spawnPosition, Quaternion.identity);

        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
        if (cameraShake != null)
        {
            StartCoroutine(cameraShake.Shake(0.2f, 0.2f)); // Adjust shake duration & intensity
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
