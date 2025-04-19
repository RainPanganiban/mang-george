using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Club : MonoBehaviour
{
    private Vector2 startPos;
    private Transform bungisngis;
    private Rigidbody2D rb;
    private bool returning = false;
    private Transform player;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxDistance = 8f;
    [SerializeField] private float rotationSpeed = 500f;

    public void Initialize(Vector2 startPosition, Transform boss)
    {
        startPos = startPosition;
        bungisngis = boss;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector2 targetDirection = Vector2.right;
        if (player != null)
        {
            targetDirection = ((Vector2)player.position - startPos).normalized;
        }

        rb.velocity = targetDirection * speed;
    }

    private void Update()
    {
        // Spin the club
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Switch to returning mode after reaching max distance
        if (!returning && Vector2.Distance(transform.position, startPos) >= maxDistance)
        {
            returning = true;
        }

        // If returning, update direction toward Bungisngis
        if (returning && bungisngis != null)
        {
            Vector2 dir = ((Vector2)bungisngis.position - rb.position).normalized;
            rb.velocity = dir * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>()?.TakeDamage(3);
        }

        if (returning && collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
