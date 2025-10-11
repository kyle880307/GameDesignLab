using UnityEngine;

public class Starman : MonoBehaviour
{
    [Header("Game Constants")]
    public GameConstants gameConstants;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public AudioSource audioSource;

    private float speed;
    private float bounceForce;
    private float lifetime;
    private int scoreValue;
    
    private float lifeTimer;
    private bool movingRight = true;
    private bool isGrounded = false;
    private GameManager gameManager;

    private void Awake()
    {
        // Get components if not assigned
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // Find GameManager
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        // Initialize values from GameConstants
        if (gameConstants != null)
        {
            speed = gameConstants.starmanSpeed;
            bounceForce = gameConstants.starmanBounceForce;
            lifetime = gameConstants.starmanLifetime;
            scoreValue = gameConstants.starmanScoreValue;
        }
        else
        {
            Debug.LogWarning("GameConstants not assigned to Starman!");
            // Fallback values
            speed = 3f;
            bounceForce = 8f;
            lifetime = 10f;
            scoreValue = 100;
        }

        // Don't auto-launch - wait for Launch() call from QnsBox
        // Initialize timer to 0 so Update() doesn't run until Launch() is called
        lifeTimer = 0f;
        
        Debug.Log("Starman Start() completed - waiting for Launch() call");
    }

    public void Launch()
    {
        Debug.Log("Starman Launch() called!");
        lifeTimer = lifetime;
        
        // Make sure the GameObject is active
        gameObject.SetActive(true);
        
        // Set initial velocity - moving right and bouncing
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(speed, bounceForce);
            Debug.Log($"Starman velocity set to: {rb.linearVelocity}");
        }
        else
        {
            Debug.LogError("Starman Rigidbody2D is null!");
        }

        // Start the starman animation
        if (animator != null)
        {
            animator.SetBool("isActive", true);
        }

        // Play spawn sound if available
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
        
        Debug.Log($"Starman launched with speed: {speed}, bounce: {bounceForce}, lifetime: {lifetime}");
    }

    private void Update()
    {
        // Only start countdown after Launch() is called
        if (lifeTimer <= 0) return; // Don't update if not launched yet
        
        // Count down lifetime
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0)
        {
            Debug.Log("Starman lifetime expired, destroying...");
            DestroyStarman();
            return;
        }

        // Handle movement
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (rb == null) return;

        // Maintain horizontal speed
        Vector2 velocity = rb.linearVelocity;
        velocity.x = movingRight ? speed : -speed;
        rb.linearVelocity = velocity;

        // Bounce when hitting ground
        if (isGrounded && velocity.y <= 0.1f)
        {
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if hitting ground
        if (IsGround(collision))
        {
            isGrounded = true;
        }

        // Check if hitting walls - change direction
        if (IsWall(collision))
        {
            movingRight = !movingRight;
            
            // Flip sprite if needed
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (movingRight ? 1 : -1);
            transform.localScale = scale;
        }

        // Check if hitting player
        if (collision.gameObject.CompareTag("Player"))
        {
            CollectStarman();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGround(collision))
        {
            isGrounded = false;
        }
    }

    private bool IsGround(Collision2D collision)
    {
        // Check if the collision is with ground layers (adjust layer numbers as needed)
        int groundLayers = (1 << 3) | (1 << 6) | (1 << 7); // Common ground layers
        return (groundLayers & (1 << collision.gameObject.layer)) != 0;
    }

    private bool IsWall(Collision2D collision)
    {
        // Check if hitting a wall by examining the collision normal
        if (collision.contacts.Length > 0)
        {
            Vector2 normal = collision.contacts[0].normal;
            // If the normal is mostly horizontal, it's a wall
            return Mathf.Abs(normal.x) > 0.7f;
        }
        return false;
    }

    private void CollectStarman()
    {
        // Give player points
        if (gameManager != null)
        {
            gameManager.IncreaseScore(scoreValue);
        }

        // Give player invincibility power
        var player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            player.StartInvincibility(lifetime); // Use starman lifetime for invincibility duration
            Debug.Log($"Player collected Starman! Invincible for {lifetime} seconds!");
        }

        // Play collection sound
        if (audioSource != null && audioSource.clip != null)
        {
            // Play a different sound for collection if you have one
            audioSource.pitch = 1.5f; // Higher pitch for collection
            audioSource.Play();
        }

        // Remove the starman
        DestroyStarman();
    }

    private void DestroyStarman()
    {
        // Stop animation
        if (animator != null)
        {
            animator.SetBool("isActive", false);
        }

        // Destroy the GameObject
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw movement direction in editor
        Gizmos.color = Color.yellow;
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        Gizmos.DrawRay(transform.position, direction * 2f);
    }
}