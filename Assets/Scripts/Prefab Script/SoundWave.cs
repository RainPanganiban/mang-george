using System.Collections;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public float stunDuration = 0.1f;
    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                StartCoroutine(ApplyStunToPlayer(player));
            }
            Destroy(gameObject);
        }
        else if (!collision.isTrigger && !collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ApplyStunToPlayer(PlayerController player)
    {
        // Disable player movement
        player.enabled = false;

        // Gray out player sprite
        SpriteRenderer playerSR = player.GetComponent<SpriteRenderer>();
        if (playerSR != null)
        {
            playerSR.color = Color.gray;
        }

        // Handle weapon component + sprite
        Transform weaponTransform = player.transform.Find("Weapon");
        Weapon weaponScript = null;
        SpriteRenderer weaponSR = null;

        if (weaponTransform != null)
        {
            weaponScript = weaponTransform.GetComponent<Weapon>();
            weaponSR = weaponTransform.GetComponent<SpriteRenderer>();

            if (weaponScript != null) weaponScript.enabled = false;
            if (weaponSR != null) weaponSR.color = Color.gray;
        }

        // Wait during stun
        yield return new WaitForSeconds(stunDuration);

        // Restore player
        player.enabled = true;
        if (playerSR != null) playerSR.color = Color.white;

        // Restore weapon
        if (weaponScript != null) weaponScript.enabled = true;
        if (weaponSR != null) weaponSR.color = Color.white;
    }
}
