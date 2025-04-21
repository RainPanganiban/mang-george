using UnityEngine;

public class EyeLaser : MonoBehaviour
{
    public float speed = 5f;
    public float duration = 2f;
    public bool moveUpwards = true; // false = top-to-bottom

    private float timer;

    void Update()
    {
        float direction = moveUpwards ? 1f : -1f;
        transform.position += Vector3.up * speed * direction * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= duration)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.TakeDamage(15f);
        }
    }
}
