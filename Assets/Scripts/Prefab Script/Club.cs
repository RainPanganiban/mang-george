using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Club : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 returnTarget;
    private Transform bungisngis;
    private Rigidbody2D rb;
    private bool returning = false;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxDistance = 8f;

    public void Initialize(Vector2 startPosition, Transform boss)
    {
        startPos = startPosition;
        bungisngis = boss;
        returnTarget = boss.position;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 dir = (returnTarget - startPos).normalized;
        rb.velocity = dir * speed;
    }

    private void Update()
    {
        if (!returning && Vector2.Distance(transform.position, startPos) >= maxDistance)
        {
            returning = true;
        }

        if (returning)
        {
            Vector2 dir = ((Vector2)bungisngis.position - rb.position).normalized;
            rb.velocity = dir * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Damage player
            collision.GetComponent<PlayerController>()?.TakeDamage(10);
        }

        if (returning && collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

}
