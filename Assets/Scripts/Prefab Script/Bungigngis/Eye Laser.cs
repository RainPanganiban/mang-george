using UnityEngine;

public class EyeLaser : MonoBehaviour
{
    public float rotationSpeed = 200f;
    public bool rotateUpwards = true;
    public bool faceRight = false;
    public float damage = 15f;

    private float totalRotated = 0f;
    private float rotationDirection;

    void Start()
    {
        rotationDirection = -1f; // Always rotate downward

        // Flip the visual sprite if facing right
        if (faceRight)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }

        // Set a gentle downward angle at the start
        float initialAngle = faceRight ? 30f : -30f;
        transform.rotation = Quaternion.Euler(0f, 0f, initialAngle);
    }

    void Update()
    {
        float rotationStep = rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, 0f, rotationStep * rotationDirection);
        totalRotated += rotationStep;

        if (totalRotated >= 75f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
