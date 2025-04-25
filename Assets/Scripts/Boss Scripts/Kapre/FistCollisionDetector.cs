using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistCollisionDetector : MonoBehaviour
{
    private KaprePhase3 boss;
    private bool hasDamagedPlayer = false;

    private void Start()
    {
        boss = FindObjectOfType<KaprePhase3>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasDamagedPlayer)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(10f);
                hasDamagedPlayer = true;

                if (boss != null)
                    boss.ShakeCamera(0.5f);
            }
        }
    }

    public void ResetFistState()
    {
        hasDamagedPlayer = false;
    }
}
