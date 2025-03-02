using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Damage : MonoBehaviour
{

    public int damage = 10;
    private int finalDamage;


    private void Start()
    {

        finalDamage = Mathf.RoundToInt(damage * PlayerStats.Instance.damageMultiplier);

    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {

        Enemy enemy = hitInfo.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
