using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
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

    [Header("Teleport Settings")]
    public float teleportInterval = 3f;
    public float minX = -7f, maxX = 7f, centerX = 0f; 
    public float groundY = -4.227f; 
    private bool isTeleporting = false;

    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        attackTimer = attackInterval;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(TeleportRoutine());
    }

    void Update()
    {
        HandlePhases();
        HandleAttacks();
    }

    IEnumerator TeleportRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(teleportInterval);
            Teleport();
        }
    }

    void Teleport()
    {
        if (isTeleporting) return;
        isTeleporting = true;

        
        float[] possiblePositions = { minX, centerX, maxX };
        float newX = possiblePositions[Random.Range(0, possiblePositions.Length)];

        
        transform.position = new Vector3(newX, groundY, 0);

        isTeleporting = false;
    }

    void HandlePhases()
    {
        if (currentHealth > 60)
        {
            currentPhase = 1;
            spriteRenderer.color = Color.white;
        }
        else if (currentHealth > 30)
        {
            currentPhase = 2;
            spriteRenderer.color = Color.blue;
        }
        else
        {
            currentPhase = 3;
            spriteRenderer.color = Color.red;
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
        switch (currentPhase)
        {
            case 1:
                HomingShot();
                break;
            case 2:
                SpreadShot();
                break;
            case 3:
                SpiralShot();
                break;
        }
    }

    void HomingShot()
    {
        if (bulletPrefab != null && firePoint != null && player != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            Vector2 direction = (player.position - firePoint.position).normalized;
            bulletRb.velocity = direction * bulletSpeed;

            Destroy(bullet, 5f);
        }
    }

    void SpreadShot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Vector3 spawnPosition = player.position + new Vector3(0, 10f, 0);
            float[] angles = { -15f, 0f, 15f };

            foreach (float angle in angles)
            {
                GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

                Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.down;
                bulletRb.velocity = direction * bulletSpeed;

                Destroy(bullet, 5f);
            }
        }
    }

    void SpiralShot()
    {
        int numBullets = 8;
        float angleStep = 360f / numBullets;
        float startAngle = Random.Range(0f, 360f);

        Vector3 randomPosition = player.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);

        for (int i = 0; i < numBullets; i++)
        {
            float angle = startAngle + (angleStep * i);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject bullet = Instantiate(bulletPrefab, randomPosition, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = direction * bulletSpeed;

            Destroy(bullet, 5f);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        slider.value = currentHealth;
       
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        
        Destroy(gameObject);
        FindAnyObjectByType<UpgradeManager>().ShowUpgradeOptions();
       
    }
}
