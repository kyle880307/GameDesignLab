using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;
    public int scoreValue = 1; // points for killing this enemy
    private EnemyEvents enemyEvents;

    public Vector3 startPosition = new Vector3(0.0f, 0.0f, 0.0f);

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        // get the starting position
        originalX = transform.position.x;
        ComputeVelocity();
    }
    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }
    void Moveyuyukoa()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    // note that this is Update(), which still works but not ideal. See below.
    void Update()
    {
        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {// move yuyukoa
            Moveyuyukoa();
        }
        else
        {
            // change direction
            moveRight *= -1;
            ComputeVelocity();
            Moveyuyukoa();
        }
    }



    void Awake()
    {
        enemyEvents = GetComponent<EnemyEvents>();
        if (enemyEvents == null) enemyEvents = gameObject.AddComponent<EnemyEvents>();
    }

    public void Kill()
    {
        enemyEvents.KillEnemy(scoreValue); // fire event
        Destroy(gameObject);
    }
    



    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name);
    }
}