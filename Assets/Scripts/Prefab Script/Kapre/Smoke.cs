using System.Collections;
using UnityEngine;

public class SmokeProjectile : MonoBehaviour
{
    public float hoverDuration = 1.5f;
    public float homingDuration = 1.5f;
    public float homingSpeed = 2f;
    public float hoverAmplitude = 0.2f;
    public float hoverFrequency = 2f;
    public float rotationSpeed = 720f;
    public float damage = 10f;

    private Transform player;
    private Vector3 homingDirection;
    private bool isActive = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(HoverAndHoming());
    }

    IEnumerator HoverAndHoming()
    {
        float elapsedHoverTime = 0f;
        Vector3 center = transform.position;

        // Hovering
        while (elapsedHoverTime < hoverDuration)
        {
            float offsetX = Mathf.Cos(elapsedHoverTime * hoverFrequency) * hoverAmplitude;
            float offsetY = Mathf.Sin(elapsedHoverTime * hoverFrequency) * hoverAmplitude;
            transform.position = center + new Vector3(offsetX, offsetY, 0f);
            elapsedHoverTime += Time.deltaTime;
            yield return null;
        }

        float elapsedHomingTime = 0f;

        // Homing
        while (elapsedHomingTime < homingDuration)
        {
            Vector3 targetDir = (player.position - transform.position).normalized;

            transform.position += targetDir * homingSpeed * Time.deltaTime;

            // Face the player
            FaceDirection(targetDir);

            homingDirection = targetDir;

            elapsedHomingTime += Time.deltaTime;
            yield return null;
        }

        // Continue in the last homing direction
        while (isActive)
        {
            transform.position += homingDirection * homingSpeed * Time.deltaTime;

            // Face travel direction
            FaceDirection(homingDirection);

            yield return null;
        }
    }

    void FaceDirection(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, 0, angle),
            rotationSpeed * Time.deltaTime
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            isActive = false;
            Destroy(gameObject); // Remove projectile
        }
    }
}
