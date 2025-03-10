using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour
{
    private Transform target;
    public float speed = 5f; // Increased speed
    private bool isAttacking = false;

    public float wanderDuration = 1.5f; // Shorter wandering phase
    public float wanderRange = 2.5f; // Slightly larger wander range
    private Vector2 wanderTarget;

    void Start()
    {
        StartCoroutine(WanderBeforeAttack());
    }

    public void Initialize(Transform playerTarget)
    {
        target = playerTarget;
    }

    void Update()
    {
        if (!isAttacking)
        {
            // Move randomly during wander phase
            transform.position = Vector2.MoveTowards(transform.position, wanderTarget, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, wanderTarget) < 0.1f)
            {
                SetNewWanderTarget();
            }
        }
        else if (target != null)
        {
            // Home in on the player after wandering
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    IEnumerator WanderBeforeAttack()
    {
        float elapsedTime = 0f;
        while (elapsedTime < wanderDuration)
        {
            SetNewWanderTarget();
            yield return new WaitForSeconds(0.4f); // Change direction more frequently
            elapsedTime += 0.4f;
        }
        isAttacking = true; // Start homing in on the player
    }

    void SetNewWanderTarget()
    {
        wanderTarget = new Vector2(
            transform.position.x + Random.Range(-wanderRange, wanderRange),
            transform.position.y + Random.Range(-wanderRange, wanderRange)
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Deal damage to the player if needed
            collision.GetComponent<PlayerController>().TakeDamage(10);
            Destroy(gameObject); // Destroy the bat on impact
        }
        else if (collision.CompareTag("Bullet")) // If the player shoots it
        {
            Destroy(gameObject);
        }
    }
}
