using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour
{
    private Transform target;
    private Manananggal boss;
    public float speed = 2f;
    private bool isAttacking = false;
    private bool isWandering = true;
    private Vector2 originalPosition;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public float wanderTime = 1f;
    public float hoverTime = 0.5f;
    public float swoopSpeed = 6f;


    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

         if (animator == null)
        {
            animator.SetTrigger("Fire");
        }
    }

    void FacePlayer()
    {
        spriteRenderer.flipX = target.position.x > transform.position.x;
    }

    public void Initialize(Transform playerTarget, Vector3 spawnPos, Manananggal bossRef)
    {
        target = playerTarget;
        boss = bossRef;
        originalPosition = spawnPos;
        StartCoroutine(WanderBeforeAttack());
    }

    IEnumerator WanderBeforeAttack()
    {
        float elapsedTime = 0f;
        while (elapsedTime < wanderTime)
        {
            Vector2 wanderTarget = new Vector2(
                originalPosition.x + Random.Range(-15f, 5f),
                originalPosition.y + Random.Range(-0.5f, 7f)
            );

            while (Vector2.Distance(transform.position, wanderTarget) > 0.1f)
            {
                if (!isWandering) yield break;
                transform.position = Vector2.MoveTowards(transform.position, wanderTarget, speed * Time.deltaTime);
                yield return null;
            }

            elapsedTime += 0.3f;
            yield return new WaitForSeconds(0.3f);
        }

        isWandering = false;
        yield return new WaitForSeconds(hoverTime);
        isAttacking = true;
    }

    void Update()
    {
        if (isAttacking && target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, swoopSpeed * Time.deltaTime);
        }

        FacePlayer();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().TakeDamage(10);
            DestroyBat();
        }
        else if (collision.CompareTag("Bullet")) // If the player shoots it
        {
            DestroyBat();
        }
    }

    void DestroyBat()
    {
        if (boss != null)
        {
            boss.OnBatDestroyed(); // Notify Manananggal
        }
        Destroy(gameObject);
    }
}
