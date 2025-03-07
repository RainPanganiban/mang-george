using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Damage : MonoBehaviour
{

    public int damage = 10;
    public float lifetime = 3f;
    public int finalDamage;


    private void Start()
    {
        Destroy(gameObject, lifetime);
        finalDamage = Mathf.RoundToInt(damage * PlayerStats.Instance.damageMultiplier);

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // Check if it hits an enemy
        {
            collision.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject); // Destroy bullet on impact
        }
        else if (!collision.CompareTag("Player")) // Ignore player collision
        {
            Destroy(gameObject);
        }
    }
}
