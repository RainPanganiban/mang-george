using UnityEngine;

public class Boulder : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool hasLanded = false;
    private Transform player;

    public float lobHeight = 5f;
    public float rollSpeed = 5f;
    public float damage = 10f;
    private bool hasDealtLobDamage = false;

    private Vector2 rollDirection;
    private bool readyToRoll = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        LaunchTowards(player.position);
    }

    void Update()
    {
        if (readyToRoll)
        {
            rb.velocity = new Vector2(rollDirection.x * rollSpeed, 0f);

            // Rotate the boulder visually
            float rotationSpeed = 500f;
            float direction = Mathf.Sign(rb.velocity.x);
            transform.Rotate(0, 0, -direction * rotationSpeed * Time.deltaTime);
        }
    }

    void LaunchTowards(Vector2 target)
    {
        Vector2 start = transform.position;
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

        float displacementX = target.x - start.x;
        float time = Mathf.Sqrt(2 * lobHeight / gravity) + Mathf.Sqrt(2 * (start.y - target.y + lobHeight) / gravity);

        float velocityX = displacementX / time;
        float velocityY = Mathf.Sqrt(2 * gravity * lobHeight);

        Vector2 launchVelocity = new Vector2(velocityX, velocityY);
        rb.velocity = launchVelocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded && collision.collider.CompareTag("Ground"))
        {
            hasLanded = true;

            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;

            Vector2 currentPlayerPosition = player.position;
            rollDirection = (currentPlayerPosition.x > transform.position.x) ? Vector2.right : Vector2.left;

            Invoke(nameof(BeginRoll), 0.05f);
        }

        if (collision.collider.CompareTag("Player"))
        {
            if (!hasLanded && !hasDealtLobDamage)
            {
                collision.collider.GetComponent<PlayerController>()?.TakeDamage(damage);
                hasDealtLobDamage = true;
                Destroy(gameObject);
            }
            else if (hasLanded)
            {
                collision.collider.GetComponent<PlayerController>()?.TakeDamage(damage);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject, 10f);
            }
        }
    }

    void BeginRoll()
    {
        readyToRoll = true;
    }
}
