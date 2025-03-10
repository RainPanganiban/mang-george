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
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Player")) // Prevents bullets from hitting the player
        {
            Destroy(gameObject);
        }
    }

}
