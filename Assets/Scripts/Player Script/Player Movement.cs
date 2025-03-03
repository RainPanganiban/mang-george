using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashSpeed = 10f;
    public float dashTime = 0.2f;
    public float dashCooldown = 2f;

    [Header("Health Settings")]
    public int playerHealth = 100;
    private int currentHealth;
    public Slider slider;
    public Slider dashSlider;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool isDashing;
    private bool isInvincible = false;
    private bool canDash = true;
    private float lastMoveDirection = 1f;
    private float dashCooldownTimer;

    void Start()
    {

        moveSpeed = PlayerStats.Instance.moveSpeed;
        dashCooldown = PlayerStats.Instance.dashCooldown;
        currentHealth = PlayerStats.Instance.currentHealth;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = playerHealth;
        slider.maxValue = playerHealth;
        slider.value = currentHealth;
        dashSlider.maxValue = dashCooldown;
        dashSlider.value = dashCooldown;
        dashCooldownTimer = dashCooldown;
    }

    void Update()
    {
        Move();
        Jump();
        UpdateDashCooldown();

        if (Input.GetKeyDown(KeyCode.Space) && canDash && !isDashing)
        {
            canDash = false;
            StartCoroutine(Dash());
        }
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (moveInput != 0)
        {
            lastMoveDirection = moveInput;
            spriteRenderer.flipX = moveInput < 0;
        }
        if (!isDashing)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        isInvincible = true;
        GetComponent<Collider2D>().enabled = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        rb.velocity = new Vector2(lastMoveDirection * dashSpeed, rb.velocity.y);
        StartCoroutine(BlinkEffect());
        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;
        isInvincible = false;
        GetComponent<Collider2D>().enabled = true;

        dashCooldownTimer = 0;
        while (dashCooldownTimer < dashCooldown)
        {
            dashCooldownTimer += Time.deltaTime;
            dashSlider.value = dashCooldownTimer;
            yield return null;
        }
        canDash = true;
    }

    void UpdateDashCooldown()
    {
        if (!canDash)
        {
            dashSlider.value = dashCooldownTimer;
        }
    }

    IEnumerator BlinkEffect()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        while (isInvincible)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        slider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        Destroy(gameObject);
    }
}
