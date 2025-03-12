using UnityEngine;

public class WindProjectile : MonoBehaviour
{
    public float speed = 6f;         // Speed of the wind gust
    public int damage = 5;           // Damage dealt
    public float knockbackForce = 7f;// Knockback strength
    public float lifetime = 2f;      // Auto-destroy time

    private Vector2 direction;

    public void Initialize(Vector3 playerPosition)
    {
        direction = (playerPosition - transform.position).normalized; // Set direction toward player
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime); // Move toward the player
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage
            other.GetComponent<IDamageable>()?.TakeDamage(damage);

            // Apply knockback
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
    }
}
