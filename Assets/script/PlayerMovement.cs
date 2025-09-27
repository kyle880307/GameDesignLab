using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 15;
    private Rigidbody2D reimuBody;
    private SpriteRenderer reimuSprite;
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

    public Animator reimuAnimator;

    public AudioClip marioDeath;
    public float deathImpulse = 15;

    // state
    [System.NonSerialized]
    public bool alive = true;
    public AudioSource marioAudio;

    public Transform gameCamera; // assign in inspector

    public BoxCollider2D boxCollider;

    public QnsBox[] qnsBoxes;

    public BrickBoxCoin[] brickBoxes;



    public void PlayJumpSound()
    {
        // play jump sound
        Debug.Log("Jump sound called!");
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void PlayDeathImpulse()
    {
        reimuBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void GameOverScene()
    {
        // stop time
        Time.timeScale = 0.0f;
        // set gameover scene
        gameOverPanel.SetActive(true); // replace this with whichever way you triggered the game over screen for Checkoff 1
    }

    void Start()
    {
        reimuSprite = GetComponent<SpriteRenderer>();
        Application.targetFrameRate = 60;
        reimuBody = GetComponent<Rigidbody2D>();
         // update animator state
        reimuAnimator.SetBool("onGround", onGroundState);
    }

    void Update()
    {
        // Flip sprite
        // Flip sprite and stop dash if moving opposite
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            reimuSprite.flipX = true;
            Vector2 offset = boxCollider.offset;
            offset.x = Mathf.Abs(offset.x); // ensure it's facing left
            boxCollider.offset = offset;
            if (isDashing || reimuBody.linearVelocity.x > 0.1f)
            {
                StopDash(true); // stop dash and play skid
            }
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            reimuSprite.flipX = false;
            Vector2 offset = boxCollider.offset;
            offset.x = -Mathf.Abs(offset.x); // ensure it's facing right
            boxCollider.offset = offset;
            if (isDashing || reimuBody.linearVelocity.x < -0.1f)
            {
                StopDash(true); // stop dash and play skid
            }
        }


        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && onGroundState)
        {
            reimuBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            // update animator state
            reimuAnimator.SetBool("onGround", onGroundState);
        }

        // Dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && Time.time >= (lastDash + dashCooldown))
        {
            isDashing = true;
            dashTimeLeft = dashTime;
            lastDash = Time.time;
            reimuAnimator.SetBool("onDash", isDashing);
        }

        // Fast drop (only in air)
        if (Input.GetKeyDown(KeyCode.S) && !onGroundState && !isDropping)
        {
            isDropping = true;
            Debug.Log("isDropping");
            reimuBody.linearVelocity = new Vector2(reimuBody.linearVelocity.x, -dropSpeed);
        }
        reimuAnimator.SetFloat("xSpeed", Mathf.Abs(reimuBody.linearVelocity.x));

        // update animator with vertical velocity
        reimuAnimator.SetFloat("ySpeed", reimuBody.linearVelocity.y);

    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            float dashDir = faceRightState ? 1f : -1f;
            reimuBody.linearVelocity = new Vector2(dashDir * dashSpeed, reimuBody.linearVelocity.y);

            dashTimeLeft -= Time.fixedDeltaTime;
            if (dashTimeLeft <= 0)
            {
                StopDash();
            }
            return; // skip normal movement while dashing
        }

        if (alive)
        {

            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            // other code
            if (Mathf.Abs(moveHorizontal) > 0)
            {
                Vector2 movement = new Vector2(moveHorizontal, 0);
                if (reimuBody.linearVelocity.magnitude < maxSpeed)
                    reimuBody.AddForce(movement * speed);
            }

            // Stop movement when key released
            if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
            {
                reimuBody.linearVelocity = new Vector2(0, reimuBody.linearVelocity.y);
            }

        }
    }

    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

    private int groundContacts = 0;

    void OnCollisionEnter2D(Collision2D col)
    {
        if ((collisionLayerMask & (1 << col.gameObject.layer)) > 0)
        {
            groundContacts++;
            onGroundState = true;
            isDropping = false;
            Debug.Log("On ground!!!");
            reimuAnimator.SetBool("onGround", true);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if ((collisionLayerMask & (1 << col.gameObject.layer)) > 0)
        {
            groundContacts--;
            if (groundContacts <= 0)
            {
                groundContacts = 0;
                onGroundState = false;
                reimuAnimator.SetBool("onGround", false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemies"))
        {
            Debug.Log("Collided with goomba!");

            // play death animation
            reimuAnimator.Play("ReimuHitAir");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;

            // // Display final score
            finalScoreText.text = "Score: " + jumpOverGoomba.score.ToString();
        }
    }
    
    private void StopDash(bool playSkid = false)
    {
        isDashing = false;
        reimuBody.linearVelocity = new Vector2(0, reimuBody.linearVelocity.y);
        reimuAnimator.SetBool("onDash", false);
        if (playSkid)
        {
            reimuAnimator.SetTrigger("onSkid");
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
        reimuBody.transform.position = new Vector3(0f, -3.662f, 0.0f);
        // reset sprite direction
        faceRightState = true;
        reimuSprite.flipX = false;
        // reset score
        scoreText.text = "Score: 0";
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }

        jumpOverGoomba.score = 0;
        qnsBoxes = FindObjectsOfType<QnsBox>();
        foreach (QnsBox box in qnsBoxes)
        {
            box.ResetBox();
        }
        brickBoxes = FindObjectsOfType<BrickBoxCoin>();
        foreach (BrickBoxCoin box in brickBoxes)
        {
            box.ResetBox();
        }


        // reset animation
        reimuAnimator.SetTrigger("gameRestart");
        alive = true;


          // reset camera position
        gameCamera.position = new Vector3(0, 0, -10);


    }
}
