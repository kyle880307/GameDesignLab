using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 15f;
    public int damage = 100; // damage dealt to boss or enemies

    private Vector3 startPos;
    private Vector2 direction;

    public void SetDirection(bool facingRight)
    {
        direction = facingRight ? Vector2.right : Vector2.left;
    }

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // If hit Enemy
     
        if (col.CompareTag("Enemies"))
        {
            EnemyMovement enemy = col.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.Kill(); // this fires the event automatically
            }
            Destroy(gameObject);
        }
    


        // If hit Boss
        Boss boss = col.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
        }

        // If hit Wall
        if (col.CompareTag("Obstacles"))
        {
            Destroy(gameObject);
        }
    }
}
