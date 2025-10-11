using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Game Constants")]
    public GameConstants gameConstants;

    private float speed;
    private float maxDistance;
    private int damage;

    private Vector3 startPos;
    private Vector2 direction;
    private float maxDistanceSquared; // Store squared distance for performance

    GameManager gameManager;

    void Awake()
    {
        // Initialize values from GameConstants
        if (gameConstants != null)
        {
            speed = gameConstants.fireballSpeed;
            maxDistance = gameConstants.fireballMaxDistance;
            damage = gameConstants.fireballDamage;
            maxDistanceSquared = maxDistance * maxDistance; // Cache squared distance
        }
        else
        {
            Debug.LogWarning("GameConstants not assigned to Fireball!");
            // Fallback values
            speed = 10f;
            maxDistance = 15f;
            damage = 10;
            maxDistanceSquared = maxDistance * maxDistance;
        }

        var mgrObj = GameObject.FindGameObjectWithTag("Manager");
        if (mgrObj != null)
            gameManager = mgrObj.GetComponent<GameManager>();

        // Fireballs are usually pooled as scene objects; don't DontDestroy unless intentional
        gameObject.SetActive(false); // deactivate by default
    }

    void OnEnable()
    {
        startPos = transform.position; // reset starting position each time it is activated
    }

    public void SetDirection(bool facingRight)
    {
        direction = facingRight ? Vector2.right : Vector2.left;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        // Use sqrMagnitude instead of Distance for better performance
        if ((transform.position - startPos).sqrMagnitude >= maxDistanceSquared)
        {
            gameObject.SetActive(false); // deactivate instead of destroying
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If hit Enemy
        if (other.CompareTag("Enemies"))
        {
            EnemyMovement enemy = other.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.OnHit(direction); // play hit animation + stun
            }
            Debug.Log("IncreaseScore: " + (gameConstants?.ScoreValue ?? 2));
            gameManager.IncreaseScore(gameConstants?.ScoreValue ?? 2);
            gameObject.SetActive(false); // deactivate
        }

        // If hit Boss
        Boss boss = other.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            gameObject.SetActive(false); // deactivate
        }

        // If hit Wall
        if (other.CompareTag("Obstacles"))
        {
            gameObject.SetActive(false); // deactivate
        }
    }
}
