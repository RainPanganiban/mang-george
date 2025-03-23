using UnityEngine;

public class WindProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 3f;
    public float knockbackForce = 5f;
    private Vector3 direction;

    public void Initialize(Vector3 playerPosition)
    {
        direction = (playerPosition - transform.position).normalized; // Set direction toward player

        // Rotate the projectile to face the direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Deal damage to the player
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(20);
            }

            // Apply knockback
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            // Destroy this object after handling the effect
            Destroy(gameObject);
        }
    }
}
