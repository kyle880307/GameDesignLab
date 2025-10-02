// using UnityEngine;

// public class Boss : MonoBehaviour
// {
//     [Header("Boss Settings")]
//     public int health = 100;
//     public float speed = 2.0f;
//     public Transform player;

//     private GameManager gameManager;

//     void Awake()
//     {
//         gameManager = GameObject.FindGameObjectWithTag("Manager")?.GetComponent<GameManager>();
//     }

//     void Update()
//     {
//         if (player != null)
//         {
//             transform.position = Vector2.MoveTowards(
//                 transform.position,
//                 player.position,
//                 speed * Time.deltaTime
//             );
//         }
//     }

//     public void TakeDamage(int damage)
//     {
//         health -= damage;
//         Debug.Log($"Boss took {damage} damage! Health = {health}");

//         if (health <= 0)
//         {
//             Die();
//         }
//     }

//     void Die()
//     {
//         Debug.Log("Boss defeated!");

//         // Option 1: End the game immediately
//         gameManager?.GameOver();

//         // Option 2: Trigger restart automatically after a short delay
//         // StartCoroutine(RestartAfterDelay(2f));

//         Destroy(gameObject);
//     }

//     // Optional: Automatically restart after boss death
//     private System.Collections.IEnumerator RestartAfterDelay(float delay)
//     {
//         yield return new WaitForSeconds(delay);
//         gameManager?.GameRestart();
//     }

//     public void Attack()
//     {
//         // TODO: Trigger attack animation and deal damage to player
//     }
//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision.gameObject.CompareTag("Player"))
//         {
//             collision.gameObject.GetComponent<PlayerMovement>()?.Die();
//         }
//     }


// }

using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Boss Settings")]
    public int health = 100;
    public float speed = 2f;
    public Transform player;
    public int scoreValue = 10;

    [Header("Default Position")]
    [Tooltip("Local start position used for resets.")]
    public Vector3 startPosition = Vector3.zero;

    private int maxHealth;

    private void Awake()
    {
        // Store max health for reset
        maxHealth = health;

        // If startPosition not set in Inspector, use current position
        if (startPosition == Vector3.zero)
            startPosition = transform.localPosition;
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Tell the player to die
            collision.gameObject.GetComponent<PlayerMovement>()?.Die();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        // if (health <= 0)
        // {
            // Optional: add score to player
            // GameManager.Instance?.AddScore(scoreValue);

            // gameObject.SetActive(false); // hide instead of destroying
        // }
    }

    // ================= Reset Boss =================
    public void ResetBoss()
    {
        health = maxHealth;
        transform.localPosition = startPosition;
        gameObject.SetActive(true);
    }
}


