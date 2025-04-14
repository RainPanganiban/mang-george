using System.Collections;
using UnityEngine;

public class StompShockwave : MonoBehaviour
{
    public float speed = 5f;
    public float maxScale = 2.5f;
    public float scaleSpeed = 1f;
    public float lifeTime = 3f;
    public float damage = 1f;

    private Vector2 direction = Vector2.right;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        // Move forward
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Gradually scale up
        if (transform.localScale.x < maxScale)
        {
            float newScale = Mathf.Min(maxScale, transform.localScale.x + scaleSpeed * Time.deltaTime);
            transform.localScale = new Vector3(newScale, newScale, 1f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Apply damage
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
                // Optional: Apply knockback here if you want
            }

            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
