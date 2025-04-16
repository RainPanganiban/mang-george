using System.Collections;
using UnityEngine;

public class StompShockwave : MonoBehaviour
{
    public float speed = 15f;
    public float maxScaleY = 2f;
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
        if (currentScale.y < maxScaleY)
        {
            float newY = Mathf.Min(maxScaleY, currentScale.y + scaleSpeed * Time.deltaTime);
            transform.localScale = new Vector3(currentScale.x, newY, currentScale.z);
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

            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}