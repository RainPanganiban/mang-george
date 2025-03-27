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
    public float attackInterval = 1.5f;
    private float attackTimer;
    public GameObject spearPrefab;
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
            }
        }
    }

    void HandleAttacks()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            animator.SetTrigger("ThrowSpear"); // Trigger animation
            attackTimer = attackInterval;
        }
    }

    // This function is triggered via an animation event
    public void ThrowSpear()
    {
        if (spearPrefab == null || player == null)
        {
            Debug.LogError("Spear Prefab or Player is missing!");
            return;
        }

        Vector2 playerPosition = (Vector2)player.transform.position + new Vector2(0.5f, 0);
        float throwHeight = 3f; // Adjust for arc

        Debug.DrawLine(firePoint.position, playerPosition, Color.red, 2f); // Debug target location

        GameObject spear = Instantiate(spearPrefab, firePoint.position, Quaternion.identity);
        Spear spearScript = spear.GetComponent<Spear>();

        if (spearScript != null)
        {
            spearScript.ThrowSpear(playerPosition, throwHeight);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;
        Debug.Log("Damge taken is" + damage);

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
