using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        PlayerController player = hitInfo.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject); // Destroy the bullet after hitting the player
        }
    }
}
