using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 15;
    private Rigidbody2D mario;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;

    // Jump
    public float upSpeed = 15;
    private bool onGroundState = true;

    // Movement
    public float maxSpeed = 60;

    // Dash
    public float dashSpeed = 30f;      // how fast to dash
    public float dashTime = 0.1f;      // how long dash lasts
    public float dashCooldown = 1f;    // cooldown between dashes
    private bool isDashing = false;
    private float dashTimeLeft;
    private float lastDash = -10f;

    public TextMeshProUGUI scoreText;
    public GameObject enemies;

    // Drop
    public float dropSpeed = 40f;   // how fast to fall
    private bool isDropping = false;

    public JumpOverGoomba jumpOverGoomba;

    public GameObject gameOverPanel;        // assign in inspector
    public TextMeshProUGUI finalScoreText;  // assign in inspector


    void Start()
    {
        marioSprite = GetComponent<SpriteRenderer>();
        Application.targetFrameRate = 60;
        mario = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Flip sprite
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && onGroundState)
        {
            mario.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
        }

        // Dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && Time.time >= (lastDash + dashCooldown))
        {
            isDashing = true;
            dashTimeLeft = dashTime;
            lastDash = Time.time;
        }

        // Fast drop (only in air)
        if (Input.GetKeyDown(KeyCode.S) && !onGroundState && !isDropping)
        {
            isDropping = true;
            mario.linearVelocity = new Vector2(mario.linearVelocity.x, -dropSpeed);
        }

    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            // Dash direction (left or right)
            float dashDir = faceRightState ? 1f : -1f;

            mario.linearVelocity = new Vector2(dashDir * dashSpeed, mario.linearVelocity.y);

            dashTimeLeft -= Time.fixedDeltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;

                // 🚀 Stop after dash (no leftover momentum)
                mario.linearVelocity = new Vector2(0, mario.linearVelocity.y);
            }
            return; // skip normal movement while dashing
        }

        // Normal horizontal movement
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(moveHorizontal) > 0)
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);
            if (mario.linearVelocity.magnitude < maxSpeed)
                mario.AddForce(movement * speed);
        }

        // Stop movement when key released
        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            mario.linearVelocity = new Vector2(0, mario.linearVelocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            onGroundState = true;
        isDropping = false; // reset drop state
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with goomba!");

            // Pause game
            Time.timeScale = 0.0f;

            // Show Game Over panel
            gameOverPanel.SetActive(true);

            // Display final score
            finalScoreText.text = "Score: " + jumpOverGoomba.score.ToString();
        }
    }

    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
        
        // hide game over panel
        gameOverPanel.SetActive(false);
    }

    private void ResetGame()
    {
        // reset position
        mario.transform.position = new Vector3(0f, -3.662f, 0.0f);
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset score
        scoreText.text = "Score: 0";
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }

        jumpOverGoomba.score = 0;

    }
}
