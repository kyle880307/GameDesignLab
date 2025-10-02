using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;
    public float maxSpeed = 60f;
    private bool facingRight = true;
    private float moveInput;

    [Header("Jump")]
    public float jumpForce = 15f;
    public float holdJumpForce = 50f;
    private bool onGround = true;
    private bool isJumpHolding = false;

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
    public AudioClip marioDeath;
    public Transform gameCamera;
    private readonly int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

    [Header("UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;

    [Header("Gameplay")]
    public GameObject enemies;
    public int score = 0;

    private int groundContacts = 0;
    private bool alive = true;

    public Shootyuyuko shootyuyuko;

    [Header("Death")]
    public float deathImpulse = 15f;


    void Start()
    {
        foreach (Transform enemy in enemies.transform)
        {
            EnemyEvents events = enemy.GetComponent<EnemyEvents>();
            if (events != null)
                events.onDeath.AddListener(AddScore);
        }
    }

    void Awake()
    {
        // Subscribe to ActionManager
        var manager = FindObjectOfType<ActionManager>();
        manager.jump.AddListener(OnJump);
        manager.jumpHold.AddListener(OnJumpHold);
        manager.moveCheck.AddListener(OnMove);
        manager.attack.AddListener(OnAttack);
        manager.dash.AddListener(OnDash);
        manager.drop.AddListener(OnDrop);
    }

    void Update()
    {
        if (!alive) return;

        HandleDashUpdate();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!alive) return;

        if (isDashing)
        {
            HandleDashMovement();
            return;
        }

        // Movement
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            if (Mathf.Abs(body.linearVelocity.x) < maxSpeed)
                body.AddForce(new Vector2(moveInput * speed, 0), ForceMode2D.Force);

            if (moveInput > 0 && !facingRight) Flip(true);
            else if (moveInput < 0 && facingRight) Flip(false);
        }
    }

    // ==============================
    // Input Event Handlers
    // ==============================
    public void OnMove(float dir) => moveInput = dir;

    public void OnJump()
    {
        if (onGround)
        {
            body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            SetGrounded(false);
        }
    }

    private bool jumpHoldUsed = false;

    public void OnJumpHold()
    {
        if (!onGround && !jumpHoldUsed)
        {
            body.AddForce(Vector2.up * holdJumpForce, ForceMode2D.Impulse);
            jumpHoldUsed = true; // mark it used for this jump
        }
    }


    public void OnDash()
    {
        if (!isDashing && Time.time >= lastDash + dashCooldown)
        {
            isDashing = true;
            dashTimeLeft = dashTime;
            lastDash = Time.time;
            animator.SetBool("onDash", true);
        }
    }

    public void OnDrop()
    {
        if (!onGround && !isDropping)
        {
            isDropping = true;
            body.linearVelocity = new Vector2(body.linearVelocity.x, -dropSpeed);
        }
    }

    public void OnAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        animator.SetBool("onAttack", true);

        var fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
        fireball.GetComponent<Fireball>().SetDirection(facingRight);
    }

    // ==============================
    // Helpers
    // ==============================
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
        isDashing = false;
        animator.SetBool("onDash", false);
    }

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
        if (grounded)
        {
            jumpHoldUsed = false; // reset so next jump can use jump hold again
        }
    }

    private bool IsGroundCollision(Collision2D col) =>
        (collisionLayerMask & (1 << col.gameObject.layer)) > 0;

    // ==============================
    // Collision
    // ==============================
    void OnCollisionEnter2D(Collision2D col)
    {
        if (IsGroundCollision(col))
        {
            groundContacts++;
            SetGrounded(true);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (IsGroundCollision(col))
        {
            groundContacts--;
            if (groundContacts <= 0) SetGrounded(false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemies") && alive)
        {
            marioAudio.PlayOneShot(marioDeath);
            HandleDeath();
        }
    }

    // ==============================
    // Death & Game Over
    // ==============================
    private void HandleDeath()
    {
        alive = false;
        body.linearVelocity = Vector2.zero;
        // body.isKinematic = true;

        animator.Play("ReimuHitAir");

        if (finalScoreText) finalScoreText.text = "Score: " + score;
    }

    private void PlayDeathImpulse()
    {
        body.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }


    private void GameOverScene()
    {
        Time.timeScale = 0.0f;
        gameOverPanel.SetActive(true);
    }

    // ==============================
    // Restart
    // ==============================
    public void RestartButtonCallback(int input)
    {
        ResetGame();
        Time.timeScale = 1.0f;
        gameOverPanel.SetActive(false);
    }

    private void ResetGame()
    {
        // reset position & direction
        body.transform.position = new Vector3(0f, -3.662f, 0.0f);
        Flip(true);

        // reset score & enemies
        scoreText.text = "Score: 0";
        // shootyuyuko.score = 0;
        foreach (Transform enemy in enemies.transform)
        {
            enemy.transform.localPosition = enemy.GetComponent<EnemyMovement>().startPosition;
        }

        // reset boxes
        foreach (QnsBox box in FindObjectsOfType<QnsBox>()) box.ResetBox();
        foreach (BrickBoxCoin box in FindObjectsOfType<BrickBoxCoin>()) box.ResetBox();

        // reset animation, state & camera
        animator.SetTrigger("gameRestart");
        alive = true;
        gameCamera.position = new Vector3(0, 0, -10);
    }

    // ==============================
    // Sounds
    // ==============================
    public void PlayJumpSound() =>
        marioAudio.PlayOneShot(marioAudio.clip);

    // Called by animation event
    public void EndAttack() => animator.SetBool("onAttack", false);
    
    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score;
    }


}
