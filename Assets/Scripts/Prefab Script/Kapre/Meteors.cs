using UnityEngine;

public class Meteor: MonoBehaviour
{
    public float speed = 5f;              // Speed of the meteor's fall
    public float damage = 20f;            // Damage dealt by the meteor
    public Vector2 direction;             // Direction of the meteor's fall (diagonal)
    public float impactRadius = 2f;       // Radius of impact where the meteor deals damage
    public LayerMask playerLayer;         // Player layer to check collisions

    private bool hasHitGround = false;

    void Start()
    {
        // Initialize meteor's direction based on Kapre's position
        Vector2 startPosition = new Vector2(Random.Range(-10f, 10f), 10f); // Meteor spawn position (off-screen top)
        transform.position = startPosition;

        // Set random diagonal direction (falling towards the player)
        direction = (Vector2.zero - startPosition).normalized; // Adjust this for the diagonal path

        // Start moving the meteor down
        Destroy(gameObject, 5f); // Destroy the meteor after 5 seconds (or when it hits the ground)
    }

    void Update()
    {
        // Move the meteor diagonally
        if (!hasHitGround)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // When the meteor hits the ground (or a player)
        if (collision.collider.CompareTag("Ground"))
        {
            // Damage player if within impact radius
            Collider2D[] playersInRadius = Physics2D.OverlapCircleAll(transform.position, impactRadius, playerLayer);
            foreach (Collider2D player in playersInRadius)
            {
                if (player.CompareTag("Player"))
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.TakeDamage(damage);
                    }
                }
            }

            // Make meteor explode or disappear (if needed)
            Destroy(gameObject);
        }
    }

    // For debugging purposes, visualize the impact radius
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
