using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public float stunDuration = 0.4f;
    private Vector2 direction;

    public GameObject player;
    public GameObject weapon;

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
                player.enabled = false;

                Transform weapon = player.transform.Find("Weapon");
                if (weapon != null) weapon.GetComponent<Weapon>().enabled = false;

                SpriteRenderer player_color = player.GetComponent<SpriteRenderer>();
                if (player_color != null) player_color.color = Color.gray;

                SpriteRenderer weapon_color = weapon.GetComponent<SpriteRenderer>();
                if (weapon_color != null) weapon_color.color = Color.gray;

                StartCoroutine(ReEnablePlayer(player, player_color, weapon));
            }

            Destroy(gameObject);
        }
        else if (!collision.isTrigger && !collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ReEnablePlayer(PlayerController player, SpriteRenderer player_color, Transform weapon)
    {
        yield return new WaitForSeconds(stunDuration);

        if (player != null)
        {
            player.enabled = true;

            // Re-enable weapon
            if (weapon != null) weapon.gameObject.SetActive(true);

            // Restore original color
            if (player_color != null) player_color.color = Color.white;
        }
    }
}
