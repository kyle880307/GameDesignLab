using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;
    public float maxSpeed = 60f;
    private float moveInput;
    private bool facingRight = true;

    [Header("Jump")]
    public float jumpForce = 15f;
    public float holdJumpForce = 50f;
    private bool onGround = true;

    [Header("Dash")]
    public float dashSpeed = 30f;
    public float dashTime = 0.1f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private float dashTimeLeft;
    private float lastDash = -10f;

    [Header("Drop")]
    public float dropSpeed = 40f;
    private bool isDropping = false;

    [Header("Attack")]
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;
    public float attackCooldown = 0.5f;
    private float lastAttackTime = -10f;

    [Header("References")]
    public Rigidbody2D body;
    public Animator animator;
    public SpriteRenderer sprite;
    public BoxCollider2D boxCollider;
    public AudioSource marioAudio;
    public AudioSource marioDeath;
    public Transform gameCamera;

    [Header("Gameplay")]
    public GameObject enemies;

    private int groundContacts = 0;
    private bool alive = true;
    private ActionManager actionManager;
    private GameManager gameManager;

    [Header("Death")]
    public float deathImpulse = 15f;

    public bool IsAlive => alive;  // Expose alive status to boss
    public Boss boss;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        actionManager = FindObjectOfType<ActionManager>();

        if (actionManager != null)
        {
            actionManager.jump.AddListener(OnJump);
            actionManager.jumpHold.AddListener(OnJumpHold);
            actionManager.moveCheck.AddListener(OnMove);
            actionManager.attack.AddListener(OnAttack);
            actionManager.dash.AddListener(OnDash);
            actionManager.drop.AddListener(OnDrop);
        }

        if (!body) Debug.LogWarning("Missing Rigidbody2D.");
        if (!animator) Debug.LogWarning("Missing Animator.");
        if (!sprite) Debug.LogWarning("Missing SpriteRenderer.");
        if (!boxCollider) Debug.LogWarning("Missing BoxCollider2D.");
    }

    private void Update()
    {
        if (!alive) return;
        HandleDashUpdate();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (!alive) return;

        if (isDashing)
        {
            HandleDashMovement();
            return;
        }

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            if (Mathf.Abs(body.linearVelocity.x) < maxSpeed * 0.98f)
                body.AddForce(new Vector2(moveInput * speed, 0), ForceMode2D.Force);

            body.linearVelocity = new Vector2(
                Mathf.Clamp(body.linearVelocity.x, -maxSpeed, maxSpeed),
                body.linearVelocity.y
            );

            if (moveInput > 0 && !facingRight) Flip(true);
            else if (moveInput < 0 && facingRight) Flip(false);
        }
    }

    // ================= Input Handlers =================
    public void OnMove(float dir) => moveInput = dir;

    public void OnJump()
    {
        if (!onGround) return;
        body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        SetGrounded(false);
    }

    public void OnJumpHold() => body.AddForce(Vector2.up * holdJumpForce, ForceMode2D.Impulse);

    public void OnDash()
    {
        if (isDashing || Time.time < lastDash + dashCooldown) return;
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        animator.SetBool("onDash", true);
    }

    public void OnDrop()
    {
        if (onGround || isDropping) return;
        isDropping = true;
        body.linearVelocity = new Vector2(body.linearVelocity.x, -dropSpeed);
    }

    public void OnAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;
        animator.SetBool("onAttack", true);

        var fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
        fireball.GetComponent<Fireball>().SetDirection(facingRight);
        fireball.GetComponent<AudioSource>().Play();
    }

    // ================= Dash =================
    private void HandleDashUpdate()
    {
        if (!isDashing) return;
        dashTimeLeft -= Time.deltaTime;
        if (dashTimeLeft <= 0) StopDash();
    }

    private void HandleDashMovement()
    {
        float dashDir = facingRight ? 1f : -1f;
        body.linearVelocity = new Vector2(dashDir * dashSpeed, body.linearVelocity.y);
    }

    private void StopDash()
    {
        if (!isDashing) return;
        isDashing = false;
        animator.SetBool("onDash", false);
    }

    // ================= Death =================
    public void Die()
    {
        if (!alive) return;

        alive = false;
        StopDash();
        body.linearVelocity = Vector2.zero;
        animator.Play("ReimuHitAir");
        // animator.SetBool("onDie", true);
        marioDeath.Play();
    }

    // Called at the end of the death animation
    public void OnDeathAnimationEnd()
    {
        gameManager?.GameOver();
    }

    private void PlayDeathImpulse()
    {
        body.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }


    public void RestartButtonCallback(int input)
    {
        gameManager?.GameRestart();
        ResetGame();
    }

    private void ResetGame()
    {
        if (body) body.transform.position = new Vector3(0f, -3.662f, 0f);
        body.linearVelocity = Vector2.zero;
        moveInput = 0f;
        isDropping = false;
        // animator.SetBool("onDie", false);
        StopDash();
        Flip(true);

        if (enemies)
        {
            foreach (Transform enemy in enemies.transform)
                enemy.GetComponent<EnemyMovement>()?.ResetEnemy();
        }
        if (boss != null)
        {
            boss.ResetBoss();
        }

        foreach (QnsBox box in FindObjectsOfType<QnsBox>()) box.ResetBox();
        foreach (BrickBoxCoin box in FindObjectsOfType<BrickBoxCoin>()) box.ResetBox();

        animator?.SetTrigger("gameRestart");
        alive = true;
        if (gameCamera) gameCamera.position = new Vector3(0, 0, -10);
    }

    // ================= Helpers =================
    private void Flip(bool faceRight)
    {
        facingRight = faceRight;
        sprite.flipX = !faceRight;
        boxCollider.offset = new Vector2(
            faceRight ? -Mathf.Abs(boxCollider.offset.x) : Mathf.Abs(boxCollider.offset.x),
            boxCollider.offset.y
        );
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("xSpeed", Mathf.Abs(body.linearVelocity.x));
        animator.SetFloat("ySpeed", body.linearVelocity.y);
        animator.SetBool("onGround", onGround);
    }

    private void SetGrounded(bool grounded)
    {
        onGround = grounded;
        isDropping = false;
        animator.SetBool("onGround", grounded);
    }

    private bool IsGroundCollision(Collision2D col) =>
        (1 << col.gameObject.layer & ((1 << 3) | (1 << 6) | (1 << 7))) != 0;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!IsGroundCollision(col)) return;
        groundContacts++;
        SetGrounded(true);
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (!IsGroundCollision(col)) return;
        groundContacts--;
        if (groundContacts <= 0) SetGrounded(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Boss") && alive)
        {
            Die();
        }
    }

    public void PlayJumpSound() => marioAudio.Play();
    public void EndAttack() => animator.SetBool("onAttack", false);
}
