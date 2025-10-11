using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 15f;
    public int damage = 10; // damage dealt to boss or enemies

    private Vector3 startPos;
    private Vector2 direction;
    public AudioSource fireaudio;

    GameManager gameManager;
    void Start(){
        startPos = transform.position;
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    public void SetDirection(bool facingRight)
    {
        direction = facingRight ? Vector2.right : Vector2.left;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If hit Enemy
        if (other.CompareTag("Enemies"))
        {
            EnemyMovement enemy = other.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.OnHit(direction); // tells enemy to play hit animation + stun
            }

            gameManager.IncreaseScore(1);
            Destroy(gameObject);
        }


        // If hit Boss
        Boss boss = other.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
        }

        // If hit Wall
        if (other.CompareTag("Obstacles"))
        {
            Destroy(gameObject);
        }
    }
}
