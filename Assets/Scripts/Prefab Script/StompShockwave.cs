using System.Collections;
using UnityEngine;

public class StompShockwave : MonoBehaviour
{
    public float speed = 10f;
    public float maxScaleY = 2f;
    public float maxScaleX = 1.2f;
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

        Vector3 currentScale = transform.localScale;

        bool changed = false;

        // Scale up Y axis
        float newY = currentScale.y;
        if (currentScale.y < maxScaleY)
        {
            newY = Mathf.Min(maxScaleY, currentScale.y + scaleSpeed * Time.deltaTime);
            changed = true;
        }

        // Slightly scale up X axis (width)
        float newX = currentScale.x;
        if (currentScale.x < maxScaleX)
        {
            newX = Mathf.Min(maxScaleX, currentScale.x + (scaleSpeed * 0.3f) * Time.deltaTime); // 0.3f to scale width slower
            changed = true;
        }

        if (changed)
        {
            transform.localScale = new Vector3(newX, newY, currentScale.z);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        else if (!collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}