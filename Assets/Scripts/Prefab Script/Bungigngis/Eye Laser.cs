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
        rotationDirection = rotateUpwards ? 1f : -1f;

        if (faceRight)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }

        if (rotateUpwards)
        {
            // Steep angle for upward rotation
            float initialAngle = faceRight ? -45f : 45f;
            transform.rotation = Quaternion.Euler(0f, 0f, initialAngle);
        }

        if (!rotateUpwards)
        {
            // Gentle angle for downward rotation
            float initialAngle = faceRight ? -10f : 10f;
            transform.rotation = Quaternion.Euler(0f, 0f, initialAngle);
        }
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
