using UnityEngine;

public class Spear : MonoBehaviour
{
    public float throwForce = 10f;
    public float gravityScale = 2f;
    private Rigidbody2D rb;
    private bool hasLanded = false;

    [Header("Hazard Settings")]
    public GameObject landingIndicatorPrefab; // Landing indicator prefab
    private GameObject landingIndicatorInstance;
    public float hazardDuration = 3f; // How long the spear stays before disappearing
    private bool isHazardous = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
    }

    public void ThrowSpear(Vector2 predictedPosition)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the spear!");
            return;
        }

        // Show the prediction indicator
        ShowLandingIndicator(predictedPosition);

        // Calculate throw velocity using physics formula
        Vector2 startPos = transform.position;
        Vector2 targetPos = predictedPosition;

        float timeToTarget = Mathf.Abs(targetPos.x - startPos.x) / throwForce;
        float verticalVelocity = (targetPos.y - startPos.y + 0.5f * Mathf.Abs(Physics2D.gravity.y) * timeToTarget * timeToTarget) / timeToTarget;

        // Set velocity
        rb.velocity = new Vector2(Mathf.Sign(targetPos.x - startPos.x) * throwForce, verticalVelocity);
    }

    void Update()
    {
        if (!hasLanded)
        {
            RotateTowardsVelocity();
        }
    }

    void RotateTowardsVelocity()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void ShowLandingIndicator(Vector2 position)
    {
        landingIndicatorInstance = Instantiate(landingIndicatorPrefab, position, Quaternion.identity);
        Destroy(landingIndicatorInstance, 1.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            StickIntoGround();
        }
    }

    void StickIntoGround()
    {
        hasLanded = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        rb.simulated = false;

        // Make it a hazard
        isHazardous = true;
        GetComponent<BoxCollider2D>().enabled = true;

        // Remove spear after a while
        Invoke(nameof(RemoveSpear), hazardDuration);
    }

    void RemoveSpear()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isHazardous && other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(10);
        }
    }
}
