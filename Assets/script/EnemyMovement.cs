using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [Header("Game Constants")]
    public GameConstants gameConstants;

    [Header("Patrol Settings")]
    [SerializeField] private int moveRight = -1;

    public Vector3 startPosition = Vector3.zero;

    private float maxOffset;
    private float enemyPatrolTime;
    private float hitStunTime;
    private float originalX;
    private int initialDirection;
    private Vector2 velocity;
    private Rigidbody2D enemyBody;
    private Animator animator;

    private bool isStunned = false;
    private bool isGameOver = false; // ⛔ new flag
    private int patrolDirection;
    
    void Start()
    {
        // Initialize values from GameConstants
        if (gameConstants != null)
        {
            maxOffset = gameConstants.enemyMaxOffset;
            enemyPatrolTime = gameConstants.enemyPatrolTime;
            hitStunTime = gameConstants.enemyHitStunTime;
        }
        else
        {
            Debug.LogWarning("GameConstants not assigned to EnemyMovement!");
            // Fallback values
            maxOffset = 5.0f;
            enemyPatrolTime = 2.0f;
            hitStunTime = 0.3f;
        }

        enemyBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (startPosition == Vector3.zero)
            startPosition = transform.localPosition;

        originalX = transform.position.x;
        initialDirection = moveRight;
        patrolDirection = moveRight;
        ComputeVelocity();
        UpdateFacing(moveRight);
    }
    void UpdateFacing(int direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    void ComputeVelocity()
    {
        velocity = new Vector2(moveRight * maxOffset / enemyPatrolTime, 0);
    }

    void MoveEnemy()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    void Update()
    {
        if (isGameOver || isStunned) return; 

        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            MoveEnemy();
        }
        else
        {
            // Change direction and move
            moveRight *= -1;
            patrolDirection = moveRight;
            ComputeVelocity();
            UpdateFacing(moveRight);
            MoveEnemy();
        }
    }

    public void ResetEnemy()
    {
        transform.localPosition = startPosition;
        originalX = transform.position.x;
        moveRight = initialDirection;
        patrolDirection = moveRight;
        ComputeVelocity();
        UpdateFacing(moveRight);
        isStunned = false;
        isGameOver = false;
    }

    public void StopEnemy()
    {
        isGameOver = true;
    }

    void OnEnable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.AddListener(ResetEnemy);
            GameManager.instance.gameOver.AddListener(StopEnemy);
        }
    }

    void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameRestart.RemoveListener(ResetEnemy);
            GameManager.instance.gameOver.RemoveListener(StopEnemy);
        }
    }
    
    // Called when hit by fireball, pass in hit direction
    public void OnHit(Vector2 hitDirection)
    {
        if (!isStunned)
            StartCoroutine(HitStunRoutine(hitDirection));
    }

    private IEnumerator HitStunRoutine(Vector2 hitDirection)
    {
        isStunned = true;

        // Face the fireball hit direction
        int hitDir = (hitDirection.x > 0) ? -1 : 1;
        UpdateFacing(hitDir);

        animator.SetTrigger("onHit");

        yield return new WaitForSeconds(hitStunTime);

        // Return to patrol facing direction
        UpdateFacing(patrolDirection);
        isStunned = false;
    }
}
