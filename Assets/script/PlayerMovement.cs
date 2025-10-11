using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerMovement : Singleton<PlayerMovement>
{
    [Header("Game Constants")]
    public GameConstants gameConstants;

    [Header("Movement")]
    private float speed;
    private float maxSpeed;
    private float moveInput;
    private bool facingRight = true;

    [Header("Jump")]
    private float jumpForce;
    private float holdJumpForce;
    private bool onGround = true;

    [Header("Dash")]
    private float dashSpeed;
    private float dashTime;
    private float dashCooldown;
    private bool isDashing = false;
    private float dashTimeLeft;
    private float lastDash = -10f;

    [Header("Drop")]
    private float dropSpeed;
    private bool isDropping = false;

    [Header("Attack")]
    public GameObject fireball; // drag the fireball in Inspector
    public Transform fireballSpawnPoint;
    private bool fireballActive => fireball.activeInHierarchy;

    // Cached components for performance
    private Fireball fireballScript;
    private AudioSource fireballAudio;

    [Header("Special Skill")]
    public GameObject specialSkill; // drag the special skill prefab in Inspector
    public Transform specialSkillSpawnPoint; // can use same as fireball or different
    private bool specialSkillUnlocked = false; // starts locked
    private bool specialSkillActive => specialSkill != null && specialSkill.activeInHierarchy;

    // Cached components for performance
    private SpecialSkillAttack specialSkillScript;
    private AudioSource specialSkillAudio;

    [Header("References")]
    public Rigidbody2D body;
    public Animator animator;
    public SpriteRenderer sprite;
    public BoxCollider2D boxCollider;
    public AudioSource marioAudio;
    public AudioSource marioDeath;
    public Transform gameCamera;

    [Header("Gameplay")]
    
    // Invincibility system for starman
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    private float blinkTimer = 0f;
    private const float BLINK_INTERVAL = 0.1f;

    private int groundContacts = 0;
    private ActionManager actionManager;
    private GameManager gameManager;
    private bool isGameOver = false;

    // Cached arrays for performance - avoid FindObjectsOfType in GameRestart
    private QnsBox[] qnsBoxes;
    private BrickBoxCoin[] brickBoxes;
    private PowerUpBox[] powerUpBoxes;

    private void Start()
    {
        // Initialize values from GameConstants
        if (gameConstants != null)
        {
            speed = gameConstants.speed;
            maxSpeed = gameConstants.maxSpeed;
            jumpForce = gameConstants.jumpForce;
            holdJumpForce = gameConstants.holdJumpForce;
            dashSpeed = gameConstants.dashSpeed;
            dashTime = gameConstants.dashTime;
            dashCooldown = gameConstants.dashCooldown;
            dropSpeed = gameConstants.dropSpeed;
        }
        else
        {
            Debug.LogWarning("GameConstants not assigned to PlayerMovement!");
        }

        gameManager = FindObjectOfType<GameManager>();
        SceneManager.activeSceneChanged += SetStartingPosition;

        // Cache component references for performance
        CacheComponentReferences();

        // Initialize special skill as inactive
        if (specialSkill != null)
        {
            specialSkill.SetActive(false);
            specialSkillUnlocked = false;
        }
    }

    private void CacheComponentReferences()
    {
        // Cache fireball components
        if (fireball != null)
        {
            fireballScript = fireball.GetComponent<Fireball>();
            fireballAudio = fireball.GetComponent<AudioSource>();
        }

        // Cache special skill components
        if (specialSkill != null)
        {
            specialSkillScript = specialSkill.GetComponent<SpecialSkillAttack>();
            specialSkillAudio = specialSkill.GetComponent<AudioSource>();
        }

        // Cache box arrays for GameRestart performance
        CacheBoxReferences();
    }

    private void CacheBoxReferences()
    {
        qnsBoxes = FindObjectsOfType<QnsBox>();
        brickBoxes = FindObjectsOfType<BrickBoxCoin>();
        powerUpBoxes = FindObjectsOfType<PowerUpBox>();
    }

    public void SetStartingPosition(Scene current, Scene next)
    {
        if (gameConstants == null) return;
        
        if (next.name == "Lab4_2")
        {
            body.transform.position = gameConstants.lab4_2StartingPosition;
        }
        else if (next.name == "Lab4")
        {
            body.transform.position = gameConstants.lab4StartingPosition;
        }
    }

    private void Update()
    {
        if (isGameOver) return;
        HandleDashUpdate();
        HandleInvincibility();
        UpdateAnimator();

        // Handle special skill input (K key)
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("K key pressed");
            OnSpecialSkill();
        }
    }

    private void FixedUpdate()
    {
        if (isGameOver) return;

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
        if (fireballActive) return; // only allow one at a time

        fireball.transform.position = fireballSpawnPoint.position;
        fireball.SetActive(true);
        
        // Use cached components instead of GetComponent calls
        if (fireballScript != null)
            fireballScript.SetDirection(facingRight);
        
        if (fireballAudio != null)
            fireballAudio.Play();

        animator.SetBool("onAttack", true);
    }

    public void OnSpecialSkill()
    {
        if (!specialSkillUnlocked || specialSkill == null) return;
        if (specialSkillActive) return; // only allow one at a time

        // Use same spawn point as fireball or different one if assigned
        Transform spawnPoint = specialSkillSpawnPoint != null ? specialSkillSpawnPoint : fireballSpawnPoint;
        
        specialSkill.transform.position = spawnPoint.position;
        specialSkill.SetActive(true);
        
        // Use cached components instead of GetComponent calls
        if (specialSkillScript != null)
        {
            specialSkillScript.SetDirection(facingRight);
        }

        // Play sound if available
        if (specialSkillAudio != null)
            specialSkillAudio.Play();

        // Trigger animation (you'll set this up in the animator)
        animator.SetBool("onSpecialAttack", true);
        
        Debug.Log("Special Skill Activated!");
    }

    // Method to unlock skills (called by PowerUpBox)
    public void UnlockSkill(string skillName)
    {
        if (skillName == "SpecialAttack")
        {
            specialSkillUnlocked = true;
            Debug.Log("Special Attack Unlocked! Press K to use it.");
        }
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

    // ================= Invincibility System =================
    public void StartInvincibility(float duration)
    {
        isInvincible = true;
        invincibilityTimer = duration;
        blinkTimer = 0f;
        Debug.Log($"Player is now invincible for {duration} seconds!");
    }

    private void HandleInvincibility()
    {
        if (!isInvincible) return;

        // Count down invincibility timer
        invincibilityTimer -= Time.deltaTime;
        if (invincibilityTimer <= 0)
        {
            EndInvincibility();
            return;
        }

        // Handle blinking effect
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= BLINK_INTERVAL)
        {
            blinkTimer = 0f;
            sprite.color = sprite.color.a > 0.5f ? new Color(1, 1, 1, 0.3f) : Color.white;
        }
    }

    private void EndInvincibility()
    {
        isInvincible = false;
        invincibilityTimer = 0f;
        sprite.color = Color.white; // Restore normal color
        Debug.Log("Player invincibility ended!");
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    // ================= Death =================
    public void Die()
    {
        // Don't die if invincible
        if (isInvincible) return;

        StopDash();
        isGameOver = true;
        if (body != null) body.linearVelocity = Vector2.zero;
        marioDeath.Play();
        animator.Play("ReimuHitAir");
    }

    // Called at the end of the death animation
    public void OnDeathAnimationEnd()
    {
        gameManager.GameOver();
    }

    private void PlayDeathImpulse()
    {
        float deathForce = gameConstants != null ? gameConstants.deathImpulse : 15f;
        body.AddForce(Vector2.up * deathForce, ForceMode2D.Impulse);
    }


    public void GameRestart()
    {
        SetStartingPosition(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());
        if (body != null) body.linearVelocity = Vector2.zero;
        moveInput = 0f;
        isDropping = false;
        isGameOver = false;
        StopDash();
        Flip(true);

        // Reset skill unlock status
        specialSkillUnlocked = false;
        if (specialSkill != null)
            specialSkill.SetActive(false);

        // Use cached arrays instead of FindObjectsOfType
        if (qnsBoxes != null)
            foreach (QnsBox box in qnsBoxes) 
                if (box != null) box.ResetBox();
        
        if (brickBoxes != null)
            foreach (BrickBoxCoin box in brickBoxes) 
                if (box != null) box.ResetBox();
        
        if (powerUpBoxes != null)
            foreach (PowerUpBox powerBox in powerUpBoxes) 
                if (powerBox != null) powerBox.ResetBox();

        animator.SetTrigger("gameRestart");
        
        // Cache camera reference instead of finding it every time
        if (gameCamera == null)
            gameCamera = GameObject.FindGameObjectWithTag("MainCamera")?.transform;
        
        if (gameCamera) 
            gameCamera.position = body.transform.position + new Vector3(0, 0, gameCamera.position.z);
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
        if (body != null)
        {
            animator.SetFloat("xSpeed", Mathf.Abs(body.linearVelocity.x));
            animator.SetFloat("ySpeed", body.linearVelocity.y);
        }
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
        if (col.gameObject.CompareTag("Boss") && !isGameOver && !isInvincible)
        {
            Debug.Log("Hit by Boss");
            Die();
        }

        // Handle enemy collisions with invincibility
        if (col.gameObject.CompareTag("Enemies") && !isGameOver)
        {
            if (isInvincible)
            {
                // If invincible, destroy the enemy instead
                EnemyMovement enemy = col.gameObject.GetComponent<EnemyMovement>();
                if (enemy != null)
                {
                    // Give points for defeating enemy
                    if (gameManager != null)
                        gameManager.IncreaseScore(gameConstants?.ScoreValue ?? 2);
                    
                    // Destroy or disable enemy
                    col.gameObject.SetActive(false);
                    Debug.Log("Enemy defeated by invincible player!");
                }
            }
            else
            {
                // If not invincible, take damage
                Debug.Log("Hit by Enemy");
                Die();
            }
        }

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

    public void PlayJumpSound() => marioAudio.Play();
    public void EndAttack() => animator.SetBool("onAttack", false);
    public void EndSpecialAttack() => animator.SetBool("onSpecialAttack", false);

}
