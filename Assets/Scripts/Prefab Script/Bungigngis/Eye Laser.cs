using UnityEngine;

public class EyeLaser : MonoBehaviour
{
    public float rotationSpeed = 200f;
    public bool rotateUpwards = false;
    public bool faceRight = false;

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
}