using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 5f;
    public float damage = 10f;
    public Vector2 direction = Vector2.right;

    private PolygonCollider2D myCollider;

    void Start()
    {
        myCollider = GetComponent<PolygonCollider2D>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the shockwave along the specified direction.
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the shockwave collides with the player.
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // Flip the visual only (assuming the default faces right)
        if (dir.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
