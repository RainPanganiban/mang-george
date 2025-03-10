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

    [Header("Phase Settings")]
    public int currentPhase = 1;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;

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
        HandleAttacks();
        FacePlayer();
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
                animator.SetInteger("Phase", 2);
                animator.Play("2nd_Attack");
            }
        }
    }

    void HandleAttacks()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = attackInterval;
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
        }

        StartCoroutine(PerformAttackAfterDelay(0.3f));
    }

    IEnumerator PerformAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        switch (currentPhase)
        {
            case 1:
                HomingPaniki();
                break;
            case 2:
                Airburst();
                break;
        }

        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isAttacking", false);
    }

    void HomingPaniki()
    {
        
    }

    void Airburst()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            
        }
    }

    public void TakeDamage(int damage)
    {
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
