using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 20f;
    public float impactRadius = 2f;
    public LayerMask playerLayer;

    private Vector2 direction = Vector2.down;
    private bool hasHitGround = false;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, 5f); // Safety destroy if it doesn’t hit anything
    }

    void Update()
    {
        if (!hasHitGround)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            hasHitGround = true;

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

            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
