using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Game Constants")]
    public GameConstants gameConstants;

    [Header("Boss Settings")]
    private int health;
    private float speed;
    public Transform player;
    private int scoreValue;

    [Header("Default Position")]
    public Vector3 startPosition = Vector3.zero;

    private int maxHealth;
    private bool isGameOver = false;
    private GameManager gameManager;

    private void Awake()
    {
        // Initialize values from GameConstants
        if (gameConstants != null)
        {
            health = gameConstants.bossHealth;
            speed = gameConstants.bossSpeed;
            scoreValue = gameConstants.bossScoreValue;
        }
        else
        {
            Debug.LogWarning("GameConstants not assigned to Boss!");
            // Fallback values
            health = 100;
            speed = 2f;
            scoreValue = 10;
        }

        var mgrObj = GameObject.FindGameObjectWithTag("Manager");
        if (mgrObj != null)
            gameManager = mgrObj.GetComponent<GameManager>();

        maxHealth = health;
        if (startPosition == Vector3.zero)
            startPosition = transform.localPosition;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isGameOver) return; // ⛔ Stop moving if game over

        if (player != null)
        {
            // Movement
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );

            // 🔄 Flip sprite based on direction
            if (player.position.x < transform.position.x)
            {
                // Face left
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (player.position.x > transform.position.x)
            {
                // Face right
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }


    public void TakeDamage(int damage)
    {
        if (isGameOver) return;

        health -= damage;
        Debug.Log($"Boss Health: {health}/{maxHealth}");
        if (health <= 0)
        {
            // Optional: add score to player
            gameManager.IncreaseScore(gameConstants?.bossScoreValue ?? 2);
            gameObject.SetActive(false); // hide instead of destroying
        }
    }

    public void ResetBoss()
    {
        Debug.Log("Boss Reset");
        health = maxHealth;
        transform.localPosition = startPosition;
        isGameOver = false;
    }

    public void StopBoss()
    {
        isGameOver = true;
    }
    
    void OnEnable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.AddListener(ResetBoss);
            GameManager.instance.gameOver.AddListener(StopBoss);
        }
    }
    
    void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(ResetBoss);
            GameManager.instance.gameOver.RemoveListener(StopBoss);
        }
    }
}
