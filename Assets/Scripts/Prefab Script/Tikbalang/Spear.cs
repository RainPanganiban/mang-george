using UnityEngine;
using System.Collections;
using Unity.PlasticSCM.Editor.WebApi;

public class Spear : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private Collider2D spearCollider;
    private bool hasLanded = false;
    private bool isHazard = false;

    public float spearHealth = 5f;
    private float currentHealth;
    public int damageBeforeLanding = 15;
    public int hazardDamage = 1;
    public float hazardDamageInterval = 0.2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spearCollider = GetComponent<Collider2D>();
        currentHealth = spearHealth;

    }

    public void ThrowSpear(Vector2 targetPosition, float height)
    {
        rb.gravityScale = 1;
        rb.bodyType = RigidbodyType2D.Dynamic;
        spearCollider.isTrigger = false; // Enable physical collision before landing

        Vector2 startPos = transform.position;
        float gravity = Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale;

        float timeToApex = Mathf.Sqrt(2 * height / gravity);
        float totalTime = timeToApex + Mathf.Sqrt(2 * (height - (targetPosition.y - startPos.y)) / gravity);

        float vx = (targetPosition.x - startPos.x) / totalTime;
        float vy = Mathf.Sqrt(2 * gravity * height);

        rb.velocity = new Vector2(vx, vy);
    }

    void Update()
    {
        if (!hasLanded && rb.velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // Deal 15 damage on direct hit and destroy the spear
                PlayerController playerHealth = collision.gameObject.GetComponent<PlayerController>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageBeforeLanding);
                }
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("Ground"))
            {
                // Land on ground and become a hazard
                hasLanded = true;
                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.gravityScale = 0;
                rb.freezeRotation = true;

                isHazard = true;

                // Change collider to trigger so player can walk through
                spearCollider.isTrigger = true;
                gameObject.layer = LayerMask.NameToLayer("Hazard");

                // Destroy the spear after 2 seconds
                Invoke(nameof(DestroySpear), 4f);
            }
        }
    }

    private float nextDamageTime = 0f;

    void OnTriggerStay2D(Collider2D other)
    {
        if (isHazard && other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            PlayerController playerHealth = other.GetComponent<PlayerController>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(hazardDamage);
                nextDamageTime = Time.time + hazardDamageInterval; // Apply cooldown
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if ( currentHealth <= 0)
        {
            Debug.Log("The Spear is Destroyed");
            DestroySpear();
        }
    }

    void DestroySpear()
    {
        isHazard = false;
        Destroy(gameObject);
    }
}
