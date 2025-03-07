using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireRate = 0.2f;
    public float bulletSpeed = 10f; // Added bullet speed

    private float nextFireTime = 0f;
    private SpriteRenderer weaponSprite;
    private Transform player;
    private SpriteRenderer playerSprite;
    private Vector3 originalWeaponPosition;

    void Start()
    {
        weaponSprite = GetComponent<SpriteRenderer>();
        player = transform.parent; // Assuming Weapon is a child of Player
        playerSprite = player.GetComponent<SpriteRenderer>();

        originalWeaponPosition = transform.localPosition; // Store the default weapon position
    }

    void Update()
    {
        RotateWeapon();

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void RotateWeapon()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Ensure it's in 2D space
        Vector3 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation ONLY to the weapon
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Flip logic
        bool shouldFlip = angle > 90 || angle < -90;
        playerSprite.flipX = shouldFlip; // Flip player when aiming left

        // Move weapon to the correct side of the player
        transform.localPosition = shouldFlip ? new Vector3(-originalWeaponPosition.x, originalWeaponPosition.y, originalWeaponPosition.z) : originalWeaponPosition;

        // Flip the weapon sprite to stay visually correct
        weaponSprite.flipY = shouldFlip;


    }

    void Shoot()
    {
        // Calculate the shooting direction
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure it's in 2D space
        Vector2 shootDirection = (mousePosition - firePoint.position).normalized;

        // Instantiate bullet at firePoint position with proper rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg));

        // Add velocity to the bullet
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = shootDirection * bulletSpeed;

        // Destroy the bullet after 3 seconds
        Destroy(bullet, 3f);
    }
}
