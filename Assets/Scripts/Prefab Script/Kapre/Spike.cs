using System.Collections;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public float pokeSpeed = 5f;               // Speed of the spike growing
    public float visibleDuration = 0.5f;       // Time the spike remains visible
    public float retractSpeed = 10f;           // Speed of the spike retracting
    public float damageAmount = 10f;           // Damage dealt by the spike to the player

    private bool hasHitPlayer = false;         // Ensure the spike only hits the player once

    private enum State { Growing, Holding, Shrinking }
    private State currentState = State.Growing;

    private Vector3 targetScale;
    private float timer;
    private float damageCooldown = 0.1f;      // Time before spike can damage player again
    private float damageTimer;

    void Start()
    {
        // Set the initial scale to be flat, so the spike is invisible at the start
        targetScale = transform.localScale;
        transform.localScale = new Vector3(targetScale.x, 0f, targetScale.z);
        damageTimer = damageCooldown;  // Initialize the damage cooldown timer
    }

    void Update()
    {
        // Update the damage timer
        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
        }

        switch (currentState)
        {
            case State.Growing:
                // Gradually grow the spike to its full size
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * pokeSpeed);

                // Once the spike is fully grown, change the state to Holding and start the timer
                if (Mathf.Abs(transform.localScale.y - targetScale.y) < 0.01f)
                {
                    currentState = State.Holding;
                    timer = visibleDuration;
                }
                break;

            case State.Holding:
                // Countdown the timer to determine when to start shrinking the spike
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    currentState = State.Shrinking;
                }
                break;

            case State.Shrinking:
                // Gradually shrink the spike until it is fully retracted
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale.x, 0f, targetScale.z), Time.deltaTime * retractSpeed);

                // Once the spike is fully retracted, destroy it
                if (transform.localScale.y <= 0.01f)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    // Detect collision with the player and apply damage
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && damageTimer <= 0f && !hasHitPlayer)
        {
            // Apply damage to the player
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
                hasHitPlayer = true;  // Ensure the spike only damages the player once
                damageTimer = damageCooldown;  // Reset the damage cooldown timer
                Debug.Log("Spike hit player and dealt damage");
            }
        }
    }

    // Reset damage flag when player exits the spike's trigger area
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hasHitPlayer = false;  // Reset hit check when the player exits the spike's trigger area
        }
    }
}
