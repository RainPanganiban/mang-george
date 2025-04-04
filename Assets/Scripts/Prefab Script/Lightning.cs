using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    private Collider2D lightningCollider;
    private Animator animator;

    void Start()
    {
        // Get references
        lightningCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

        // Disable collider at start
        lightningCollider.enabled = false;
        lightningCollider.isTrigger = true;
    }

    // Called by animation event to enable the collider
    public void ActivateCollider()
    {
        lightningCollider.enabled = true;
        
    }

    // Detect collision with player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(10);
            }
        }

        if (other.CompareTag("Bullet"))
        {
            return; // Do nothing, let it pass through
        }
    }

    // Called by animation event at the end to destroy lightning
    public void DestroyLightning()
    {
        Destroy(gameObject);
    }
}

