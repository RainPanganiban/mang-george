using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class ManananggalUpperBody : MonoBehaviour, IDamageable
{
    public int maxHealth = 50;
    private int currentHealth;
    //public UnityEngine.UI.Slider bossHealthSlider;
    private Animator animator;
    private Transform player;

    [Header("Movement Settings")]
    public float ascendHeight = 5f;
    public float ascendSpeed = 2f;
    public float moveSpeed = 2f;
    public float moveRange = 3f;

    private bool hasAscended = false;
    private float randomOffset;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        //bossHealthSlider.maxValue = maxHealth;
        //bossHealthSlider.value = maxHealth;

        // Assign spriteRenderer before using it
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }

        randomOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (!hasAscended)
        {
            AscendAbovePlayer();
        }
        else
        {
            MoveRandomlyAbovePlayer();
        }

        FacePlayer();
    }

    void FacePlayer()
    {
        if (player == null) return;

        // Flip sprite based on player's position
        spriteRenderer.flipX = player.position.x > transform.position.x;
    }

    void AscendAbovePlayer()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(player.position.x, player.position.y + ascendHeight, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, ascendSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            hasAscended = true;
        }
    }

    void MoveRandomlyAbovePlayer()
    {
        if (player == null) return;

        float xOffset = Mathf.Sin(Time.time + randomOffset) * moveRange;
        Vector3 targetPosition = new Vector3(player.position.x + xOffset, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        /*if (bossHealthSlider != null)
        {
            bossHealthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogWarning("Health slider not assigned!");
        }*/

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.07f);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
