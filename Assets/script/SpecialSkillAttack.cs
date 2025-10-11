using UnityEngine;

public class SpecialSkillAttack : MonoBehaviour
{
    [Header("Game Constants")]
    public GameConstants gameConstants;

    [Header("Skill Settings")]
    public LayerMask enemyLayers;

    private float speed;
    private float lifetime;
    private int damage;

    [Header("Visual")]
    public SpriteRenderer sprite;
    public Animator animator;
    public TrailRenderer trail;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private float lifeTimer;

    private Vector2 direction;

    private void Awake()
    {
        // Initialize values from GameConstants
        if (gameConstants != null)
        {
            speed = gameConstants.specialSkillSpeed;
            lifetime = gameConstants.specialSkillLifetime;
            damage = gameConstants.specialSkillDamage;
        }
        else
        {
            Debug.LogWarning("GameConstants not assigned to SpecialSkillAttack!");
            // Fallback values
            speed = 20f;
            lifetime = 3f;
            damage = 100;
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void OnEnable()
    {
        lifeTimer = lifetime;
        // Set initial velocity
        if (rb != null)
        {
            float direction = movingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * speed, 0f);
        }
    }

    private void Update()
    {
        // Count down lifetime
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetDirection(bool faceRight)
    {
        movingRight = faceRight;
        
        // Flip sprite
        if (sprite != null)
            sprite.flipX = !faceRight;

        // Set velocity
        if (rb != null)
        {
            float direction = faceRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * speed, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        // If hit Enemy
        if (collision.CompareTag("Enemies"))
        {
            EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                gameObject.SetActive(false); // deactivate
                return; // exit early
            }
        }

        // If hit Boss - Check for Boss component
        Boss boss = collision.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            gameObject.SetActive(false); // deactivate
            return; // exit early
        }

        // If hit Wall
        if (collision.CompareTag("Obstacles"))
        {
            gameObject.SetActive(false); // deactivate
        }
    }


    private void OnDisable()
    {
        // Reset state when deactivated
        lifeTimer = lifetime;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}
