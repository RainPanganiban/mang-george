using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public float stunDuration = 0.4f;
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
                // Disable player movement
                player.enabled = false;

                // Disable weapon (assumes weapon is child GameObject named "Weapon")
                Transform weapon = player.transform.Find("Weapon");
                if (weapon != null) weapon.gameObject.SetActive(false);

                // Change color to gray
                SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = Color.gray;

                StartCoroutine(ReEnablePlayer(player, sr, weapon));
            }

            Destroy(gameObject);
        }
        else if (!collision.isTrigger && !collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ReEnablePlayer(PlayerController player, SpriteRenderer sr, Transform weapon)
    {
        yield return new WaitForSeconds(stunDuration);

        if (player != null)
        {
            player.enabled = true;

            // Re-enable weapon
            if (weapon != null) weapon.gameObject.SetActive(true);

            // Restore original color
            if (sr != null) sr.color = Color.white;
        }
    }
}
