using System;
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

    private TrailRenderer trailRenderer;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool isDashing;
    private bool isDead = false;
    public bool isInvincible = false;
    private bool canDash = true;
    private float lastMoveDirection = 1f;
    private float dashCooldownTimer;
    private Color originalColor;
    private AudioManager audioManager;

    void Start()
    {

        moveSpeed = PlayerStats.Instance.moveSpeed;
        dashCooldown = PlayerStats.Instance.dashCooldown;
        currentHealth = PlayerStats.Instance.currentHealth;

        trailRenderer = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioManager = FindObjectOfType<AudioManager>();

        currentHealth = playerHealth;
        slider.maxValue = playerHealth;
        slider.value = currentHealth;
        dashSlider.maxValue = dashCooldown;
        dashSlider.value = dashCooldown;
        dashCooldownTimer = dashCooldown;
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        Move();
        Jump();
        UpdateDashCooldown();

        animator.SetFloat("yVelocity", rb.velocity.y);

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
        }
        if (!isDashing)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
            animator.SetFloat("yVelocity", rb.velocity.y);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            audioManager.PlaySFX(audioManager.jump);

            if (!animator.GetBool("isJumping")) // Set only if it's not already playing
            {
                animator.SetBool("isJumping", true);
            }

        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        isInvincible = true;
        trailRenderer.enabled = true;
        audioManager.PlaySFX(audioManager.dash);
        GetComponent<Collider2D>().enabled = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        rb.velocity = new Vector2(lastMoveDirection * dashSpeed, rb.velocity.y);
        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;
        isInvincible = false;
        GetComponent<Collider2D>().enabled = true;

        yield return new WaitForSeconds(0.1f);

        trailRenderer.Clear();
        trailRenderer.enabled = false;

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);

        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        slider.value = currentHealth;

        StartCoroutine(Flashred());

        AudioClip[] hurtSounds = { audioManager.hurt1, audioManager.hurt2, audioManager.hurt3 };
        int randomIndex = UnityEngine.Random.Range(0, hurtSounds.Length);
        audioManager.PlaySFX(hurtSounds[randomIndex]);

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    IEnumerator Flashred()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.07f);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        if (isDead) return; // Prevent multiple triggers
        isDead = true;

        // Set Animator to play Death animation
        animator.SetTrigger("isDead");

        // Disable player movement & actions
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Stops movement

        // Destroy the child object "Weapon"
        Transform weaponTransform = transform.Find("Weapon");
        if (weaponTransform != null)
        {
            Destroy(weaponTransform.gameObject);
        }
    }
}
