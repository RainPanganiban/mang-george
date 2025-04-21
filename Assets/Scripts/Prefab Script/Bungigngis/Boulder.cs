using UnityEngine;

public class Boulder : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool hasLanded = false;
    private Transform player;
    public float lobForce = 10f;
    public float rollSpeed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Launch the boulder in an arc
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 force = new Vector2(direction.x, 1f).normalized * lobForce;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded && collision.collider.CompareTag("Ground"))
        {
            hasLanded = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;

            // Start rolling
            Vector2 rollDir = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;
            rb.velocity = rollDir * rollSpeed;
        }

        if (hasLanded && collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerController>()?.TakeDamage(10);
            Destroy(gameObject);
        }
    }
}
