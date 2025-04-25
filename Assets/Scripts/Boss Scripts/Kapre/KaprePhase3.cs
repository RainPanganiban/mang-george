using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KaprePhase3 : MonoBehaviour, IDamageable
{
    public string sceneToLoad = "EndingScene";
    [SerializeField]
    [Header("Health Settings")]
    public int maxHealth = 600;
    private float currentHealth;
    public Slider slider;

    [SerializeField]
    [Header("Attack Settings")]
    public float attackInterval = 5f;
    private float attackTimer;
    public float attackCooldownTime = 2f;
    private bool canAttack = true;

    [SerializeField]
    [Header("Phase 1")]
    public GameObject leftFist;
    public GameObject rightFist;
    public float fistSpeed = 20f;
    public float fistPauseDuration = 0.3f;
    public float fistRetractSpeed = 15f;

    [SerializeField]
    [Header("Phase Settings")]
    public int currentPhase = 3;
    private bool isTransitioning = false;
    public float duration = 3f;
    //private bool hasFistsCollided = false;
    private Vector3 initialLeftFistPosition;
    private Vector3 initialRightFistPosition;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;
    private bool isInvincible = false;
    private bool isAttacking = false;

    void Start()
    {
        currentHealth = 250;
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        attackTimer = attackInterval;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        originalColor = spriteRenderer.color;

        initialLeftFistPosition = leftFist.transform.position;
        initialRightFistPosition = rightFist.transform.position;
    }

    void Update()
    {
        HandlePhases();

        if (!isTransitioning && !isAttacking)
        {
            HandleAttacks();
        }
    }

    void FacePlayer()
    {
        if (player == null) return;

        bool isFacingRight = player.position.x > transform.position.x;
        spriteRenderer.flipX = player.position.x > transform.position.x;
    }

    void HandlePhases()
    {
        if (currentHealth > 450 && currentPhase != 1)
        {
            currentPhase = 1;
            animator.SetInteger("Phase", currentPhase);
        }
        else if (currentHealth <= 450 && currentHealth > 250 && currentPhase != 2)
        {
            currentPhase = 2;
            animator.SetInteger("Phase", currentPhase);
        }
        else if (currentHealth <= 250 && currentPhase != 3)
        {
            currentPhase = 3;
            animator.SetInteger("Phase", currentPhase);
        }
    }

    void HandleAttacks()
    {
        if (!canAttack) return;

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            switch (currentPhase)
            {
                case 1:
                    // Phase 1 attacks (if any)
                    break;

                case 2:


                case 3:
                    StartCoroutine(HoverSlamAttack()); // Only this attack in Phase 3
                    break;
            }
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        Debug.Log("Attack Cooldown started");
        yield return new WaitForSeconds(attackCooldownTime);
        attackTimer = attackInterval;
        canAttack = true;
        Debug.Log("Attack Cooldown ended, ready to attack again");
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        slider.value = currentHealth;

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
            Die();
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
        SceneManager.LoadScene(sceneToLoad);

    }

    public void ShakeCamera(float duration)
    {
        float magnitude = 0.3f;
        StartCoroutine(CameraShakeCoroutine(duration, magnitude));
    }

    IEnumerator CameraShakeCoroutine(float duration, float magnitude)
    {
        Transform cam = Camera.main.transform;
        Vector3 originalPos = cam.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            cam.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.localPosition = originalPos;
    }

    /*public void TriggerFistCrushAttack()
    {
        if (!isAttacking)
            StartCoroutine(FistCrushAttack());
    }

    IEnumerator FistCrushAttack()
    {
        isAttacking = true;

        leftFist.SetActive(true);
        rightFist.SetActive(true);

        Rigidbody2D leftRb = leftFist.GetComponent<Rigidbody2D>();
        Rigidbody2D rightRb = rightFist.GetComponent<Rigidbody2D>();

        Vector2 leftDirection = (player.position - leftFist.transform.position).normalized;
        Vector2 rightDirection = (player.position - rightFist.transform.position).normalized;

        leftRb.velocity = leftDirection * fistSpeed;
        rightRb.velocity = rightDirection * fistSpeed;

        // Wait until they collide (handled in OnCollisionEnter2D)
        yield return new WaitUntil(() => hasFistsCollided);

        ShakeCamera(0.5f);
        yield return new WaitForSeconds(fistPauseDuration);

        // Retract
        Vector3 leftStart = initialLeftFistPosition;
        Vector3 rightStart = initialRightFistPosition;

        Camera cam = Camera.main;
        float screenLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        float screenRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

        while (Vector3.Distance(leftFist.transform.position, leftStart) > 0.05f)
        {
            Vector3 newLeftPos = Vector3.MoveTowards(leftFist.transform.position, leftStart, fistRetractSpeed * Time.deltaTime);
            Vector3 newRightPos = Vector3.MoveTowards(rightFist.transform.position, rightStart, fistRetractSpeed * Time.deltaTime);

            // Clamp x-positions within screen bounds
            newLeftPos.x = Mathf.Clamp(newLeftPos.x, screenLeft, screenRight);
            newRightPos.x = Mathf.Clamp(newRightPos.x, screenLeft, screenRight);

            leftFist.transform.position = newLeftPos;
            rightFist.transform.position = newRightPos;

            yield return null;
        }

        leftFist.SetActive(false);
        rightFist.SetActive(false);
        hasFistsCollided = false;
        isAttacking = false;
        StartCoroutine(AttackCooldown());
    }


    public void FistsCollided()
    {
        hasFistsCollided = true;

        leftFist.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        rightFist.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }*/

    IEnumerator HoverSlamAttack()
    {
        isAttacking = true;

        // Debug: Log positions to check if distances are calculated correctly
        Debug.Log("Left Fist Position: " + leftFist.transform.position);
        Debug.Log("Right Fist Position: " + rightFist.transform.position);
        Debug.Log("Player Position: " + player.position);

        // Calculate distances to the player
        float distLeft = Vector3.Distance(player.position, leftFist.transform.position);
        float distRight = Vector3.Distance(player.position, rightFist.transform.position);

        Debug.Log("Distance to Left Fist: " + distLeft);
        Debug.Log("Distance to Right Fist: " + distRight);

        // Choose the closest fist based on distance
        GameObject closestFist;
        Vector3 initialPosition;

        if (distLeft < distRight)
        {
            closestFist = leftFist;
            initialPosition = initialLeftFistPosition;
        }
        else
        {
            closestFist = rightFist;
            initialPosition = initialRightFistPosition;
        }

        // Debug: Log which fist was selected
        Debug.Log("Selected Fist: " + closestFist.name);

        closestFist.SetActive(true);
        Rigidbody2D fistRb = closestFist.GetComponent<Rigidbody2D>();

        // Smooth move above player
        BoxCollider2D fistCollider = closestFist.GetComponent<BoxCollider2D>();
        float fistHeight = (fistCollider != null) ? fistCollider.size.y + 5 * closestFist.transform.lossyScale.y : 0f;
        float hoverHeight = fistHeight / 2f + 0.5f; // 0.5f extra spacing

        Vector3 hoverTarget = new Vector3(player.position.x, player.position.y + hoverHeight, closestFist.transform.position.z);

        float moveTime = 0.5f;
        float elapsed = 0f;
        Vector3 startHover = closestFist.transform.position;

        while (elapsed < moveTime)
        {
            float t = elapsed / moveTime;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            closestFist.transform.position = Vector3.Lerp(startHover, hoverTarget, smoothT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        closestFist.transform.position = hoverTarget;
        yield return new WaitForSeconds(1.2f); // Hover delay

        // Slam down
        Vector3 slamTarget = new Vector3(player.position.x, player.position.y - 0.5f, closestFist.transform.position.z);
        elapsed = 0f;
        float slamTime = 0.3f;
        Vector3 startSlam = closestFist.transform.position;

        while (elapsed < slamTime)
        {
            float t = elapsed / slamTime;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            closestFist.transform.position = Vector3.Lerp(startSlam, slamTarget, smoothT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        closestFist.transform.position = slamTarget;

        // Damage player
        if (player.TryGetComponent(out PlayerController pc))
            pc.TakeDamage(1);

        ShakeCamera(0.5f);

        // Retract fist smoothly
        while (Vector3.Distance(closestFist.transform.position, initialPosition) > 0.05f)
        {
            Vector3 newPos = Vector3.MoveTowards(closestFist.transform.position, initialPosition, fistRetractSpeed * Time.deltaTime);
            closestFist.transform.position = newPos;
            yield return null;
        }

        closestFist.SetActive(false);
        isAttacking = false;

        StartCoroutine(AttackCooldown());
    }



}
