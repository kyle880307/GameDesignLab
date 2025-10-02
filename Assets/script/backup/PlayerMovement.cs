// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.InputSystem;
// using UnityEngine;
// using TMPro;

// public class PlayerMovement : MonoBehaviour
// {
//     // ==============================
//     // Movement Settings
//     // ==============================
//     [Header("Movement")]
//     public float speed = 15f;
//     public float maxSpeed = 60f;
//     private bool faceRightState = true;

//     // Jump
//     [Header("Jump")]
//     public float upSpeed = 15f;
//     private bool onGroundState = true;

//     // Dash
//     [Header("Dash")]
//     public float dashSpeed = 30f;
//     public float dashTime = 0.1f;
//     public float dashCooldown = 1f;
//     private bool isDashing = false;
//     private float dashTimeLeft;
//     private float lastDash = -10f;

//     // Drop
//     [Header("Drop")]
//     public float dropSpeed = 40f;
//     private bool isDropping = false;

//     // Attack
//     [Header("Attack")]
//     public float attackCooldown = 0.5f;   // time between attacks
//     // private bool isAttacking = false;
//     private float lastAttackTime = -10f;

//     // ==============================
//     // References
//     // ==============================
//     [Header("References")]
//     public Rigidbody2D reimuBody;
//     public SpriteRenderer reimuSprite;
//     public Animator reimuAnimator;
//     public BoxCollider2D boxCollider;
//     public Transform gameCamera;
//     public GameObject fireballPrefab;
//     public Transform fireballSpawnPoint;
//     private Fireball fireballScript;

//     [Header("UI")]
//     public TextMeshProUGUI scoreText;
//     public TextMeshProUGUI finalScoreText;
//     public GameObject gameOverPanel;

//     [Header("Audio")]
//     public AudioSource marioAudio;
//     public AudioClip marioDeath;

//     [Header("Gameplay")]
//     public GameObject enemies;
//     public JumpOverGoomba jumpOverGoomba;
//     public QnsBox[] qnsBoxes;
//     public BrickBoxCoin[] brickBoxes;

//     [Header("Death")]
//     public float deathImpulse = 15f;

//     // ==============================
//     // State
//     // ==============================
//     [System.NonSerialized] public bool alive = true;
//     private int groundContacts = 0;
//     private readonly int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

//     // ==============================
//     // Unity Methods
//     // ==============================
//     void Start()
//     {
//         Application.targetFrameRate = 60;
//         reimuAnimator.SetBool("onGround", onGroundState);
//     }

//     void Update()
//     {
//         HandleDash();
//         HandleDrop();
//         HandleAttack();
//         UpdateAnimator();
//     }

//     void FixedUpdate()
//     {
//         if (isDashing)
//         {
//             HandleDashMovement();
//             return;
//         }

//         if (alive) HandleMovement();
//     }

//     void OnCollisionEnter2D(Collision2D col)
//     {
//         if (IsGroundCollision(col))
//         {
//             groundContacts++;
//             SetGrounded(true);
//         }
//     }

//     void OnCollisionExit2D(Collision2D col)
//     {
//         if (IsGroundCollision(col))
//         {
//             groundContacts--;
//             if (groundContacts <= 0) SetGrounded(false);
//         }
//     }

//     void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Enemies"))
//         {
//             HandleDeath();
//         }
//     }

//     // ==============================
//     // Input Handlers
//     // ==============================
//     public void OnMove(InputValue input)
//     {
//         if (input.Get() == null)
//         {
//             Debug.Log("Move released");
//         }
//         else
//         {                                    
//             if (input.Get<float>() < 0 && faceRightState)
//             {
//                 FlipSprite(false);
//                 if (isDashing || reimuBody.linearVelocity.x > 0.1f) StopDash(true);
//             }

//             if (input.Get<float>() > 0 && !faceRightState)
//             {
//                 FlipSprite(true);
//                 if (isDashing || reimuBody.linearVelocity.x < -0.1f) StopDash(true);
//             }
//         }
//     }

//     private void HandleAttack()
//     {
//         if (Input.GetKeyDown(KeyCode.J) && Time.time >= lastAttackTime + attackCooldown)
//         {
//             lastAttackTime = Time.time;

//             // Create a clone of the prefab
//             GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);

//             // Pass direction to fireball
//             Fireball fb = fireball.GetComponent<Fireball>();
//             fb.SetDirection(faceRightState);

//             reimuAnimator.SetBool("onAttack", true);
//         }
//     }



//     // Called by animation event
//     public void EndAttack()
//     {
//         // isAttacking = false;
//         reimuAnimator.SetBool("onAttack", false);
//     }

//     public void OnJump()
//     {
//         if (onGroundState)
//         {
//             reimuBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
//             SetGrounded(false);
//         }
//     }

//     public void OnJumphold(InputValue value)
//     {
//         reimuBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
//     }

//     private void HandleDash()
//     {
//         if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && Time.time >= lastDash + dashCooldown)
//         {
//             isDashing = true;
//             dashTimeLeft = dashTime;
//             lastDash = Time.time;
//             reimuAnimator.SetBool("onDash", true);
//         }
//     }

//     private void HandleDrop()
//     {
//         if (Input.GetKeyDown(KeyCode.S) && !onGroundState && !isDropping)
//         {
//             isDropping = true;
//             reimuBody.linearVelocity = new Vector2(reimuBody.linearVelocity.x, -dropSpeed);
//         }
//     }

//     // Called via Animation Event at the frame where the fireball should be fired
//     public void SpawnFireball()
//     {
//         Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
//     }

//     // ==============================
//     // Movement Logic
//     // ==============================
//     private void HandleMovement()
//     {
//         float moveHorizontal = Input.GetAxisRaw("Horizontal");

//         if (Mathf.Abs(moveHorizontal) > 0)
//         {
//             if (reimuBody.linearVelocity.magnitude < maxSpeed)
//             {
//                 reimuBody.AddForce(new Vector2(moveHorizontal, 0) * speed);
//             }
//         }

//         if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
//         {
//             reimuBody.linearVelocity = new Vector2(0, reimuBody.linearVelocity.y);
//         }
//     }

//     private void HandleDashMovement()
//     {
//         float dashDir = faceRightState ? 1f : -1f;
//         reimuBody.linearVelocity = new Vector2(dashDir * dashSpeed, reimuBody.linearVelocity.y);

//         dashTimeLeft -= Time.fixedDeltaTime;
//         if (dashTimeLeft <= 0) StopDash();
//     }

//     // ==============================
//     // Helpers
//     // ==============================
//     private void FlipSprite(bool faceRight)
//     {
//         // Update facing direction
//         faceRightState = faceRight;

//         // Flip sprite by toggling only once
//         reimuSprite.flipX = !faceRight;

//         // Adjust collider offset based on direction
//         boxCollider.offset = new Vector2(
//         faceRight ? -Mathf.Abs(boxCollider.offset.x) : Mathf.Abs(boxCollider.offset.x),
//         boxCollider.offset.y
//     );
//     }

//     private void SetGrounded(bool grounded)
//     {
//         onGroundState = grounded;
//         isDropping = false;
//         reimuAnimator.SetBool("onGround", grounded);
//     }

//     private bool IsGroundCollision(Collision2D col) =>
//         (collisionLayerMask & (1 << col.gameObject.layer)) > 0;

//     private void StopDash(bool playSkid = false)
//     {
//         isDashing = false;
//         reimuBody.linearVelocity = new Vector2(0, reimuBody.linearVelocity.y);
//         reimuAnimator.SetBool("onDash", false);
//         if (playSkid) reimuAnimator.SetTrigger("onSkid");
//     }

//     private void UpdateAnimator()
//     {
//         reimuAnimator.SetFloat("xSpeed", Mathf.Abs(reimuBody.linearVelocity.x));
//         reimuAnimator.SetFloat("ySpeed", reimuBody.linearVelocity.y);
//     }

//     // ==============================
//     // Death & Game Over
//     // ==============================
//     private void HandleDeath()
//     {
//         reimuAnimator.Play("ReimuHitAir");
//         marioAudio.PlayOneShot(marioDeath);
//         alive = false;

//         finalScoreText.text = "Score: " + jumpOverGoomba.score.ToString();
//     }

//     private void PlayDeathImpulse() =>
//         reimuBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);

//     private void GameOverScene()
//     {
//         Time.timeScale = 0.0f;
//         gameOverPanel.SetActive(true);
//     }

//     // ==============================
//     // Restart
//     // ==============================
//     public void RestartButtonCallback(int input)
//     {
//         ResetGame();
//         Time.timeScale = 1.0f;
//         gameOverPanel.SetActive(false);
//     }

//     private void ResetGame()
//     {
//         // reset position & direction
//         reimuBody.transform.position = new Vector3(0f, -3.662f, 0.0f);
//         FlipSprite(true);

//         // reset score & enemies
//         scoreText.text = "Score: 0";
//         jumpOverGoomba.score = 0;
//         foreach (Transform enemy in enemies.transform)
//         {
//             enemy.transform.localPosition = enemy.GetComponent<EnemyMovement>().startPosition;
//         }

//         // reset boxes
//         foreach (QnsBox box in FindObjectsOfType<QnsBox>()) box.ResetBox();
//         foreach (BrickBoxCoin box in FindObjectsOfType<BrickBoxCoin>()) box.ResetBox();

//         // reset animation, state & camera
//         reimuAnimator.SetTrigger("gameRestart");
//         alive = true;
//         gameCamera.position = new Vector3(0, 0, -10);
//     }

//     // ==============================
//     // Sounds
//     // ==============================
//     public void PlayJumpSound() =>
//         marioAudio.PlayOneShot(marioAudio.clip);
// }
