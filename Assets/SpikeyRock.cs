using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeyRock : MonoBehaviour
{
    public float damage = 10f;
    public bool canDealDamage = false;
    private Collider2D rockCollider;

    void Start()
    {
        rockCollider = GetComponent<Collider2D>();
        rockCollider.enabled = false;
    }

    // Called via animation event
    public void EnableDamage()
    {
        canDealDamage = true;
        rockCollider.enabled = true; // Enable collision
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDealDamage && collision.CompareTag("Player"))
        {
            IDamageable player = collision.GetComponent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    // Called via animation event
    public void DestroyRock()
    {
        Destroy(gameObject);
    }
}
