using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float maxOffset = 5.0f;
    [SerializeField] private float enemyPatrolTime = 2.0f;
    [SerializeField] private int moveRight = -1; // -1 = left, 1 = right
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private float hitStunTime = 0.3f; // how long enemy is stunned after being hit

    [Tooltip("Local start position used for resets.")]
    public Vector3 startPosition = Vector3.zero;

    private float originalX;
    private int initialDirection;
    private Vector2 velocity;
    private Rigidbody2D enemyBody;
    private EnemyEvents enemyEvents;
    private Animator animator;

    private bool isStunned = false;
    private int patrolDirection; // remembers current patrol direction

    void Awake()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        enemyEvents = GetComponent<EnemyEvents>() ?? gameObject.AddComponent<EnemyEvents>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (startPosition == Vector3.zero)
            startPosition = transform.localPosition;

        originalX = transform.position.x;
        initialDirection = moveRight;
        patrolDirection = moveRight;
        ComputeVelocity();
        UpdateFacing(moveRight); // Ensure correct facing at start
    }

    void ComputeVelocity()
    {
        velocity = new Vector2(moveRight * maxOffset / enemyPatrolTime, 0);
    }

    void MoveEnemy()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        if (isStunned) return; // Don't move while stunned

        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            MoveEnemy();
        }
        else
        {
            moveRight *= -1;
            patrolDirection = moveRight;
            ComputeVelocity();
            UpdateFacing(moveRight);
            MoveEnemy();
        }
    }

    void UpdateFacing(int dir)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (dir > 0 ? 1 : -1);
        transform.localScale = scale;
    }

    public void Kill()
    {
        enemyEvents?.KillEnemy(scoreValue);
        gameObject.SetActive(false);
    }

    public void ResetEnemy()
    {
        gameObject.SetActive(true);
        transform.localPosition = startPosition;
        originalX = transform.position.x;
        moveRight = initialDirection;
        patrolDirection = moveRight;
        ComputeVelocity();
        UpdateFacing(moveRight);
        isStunned = false;
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

        if (animator != null)
            animator.SetTrigger("onHit");

        yield return new WaitForSeconds(hitStunTime);

        // Return to patrol facing direction
        UpdateFacing(patrolDirection);
        isStunned = false;
    }
}
