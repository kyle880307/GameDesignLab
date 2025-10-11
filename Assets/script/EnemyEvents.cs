using UnityEngine;
using UnityEngine.Events;

// Simple event container so other systems (e.g. PlayerMovement) can respond
// to enemy deaths without tight coupling.
public class EnemyEvents : MonoBehaviour
{
    [System.Serializable]
    public class EnemyKilledEvent : UnityEvent<int> { } // passes score value

    public EnemyKilledEvent enemyKilled = new EnemyKilledEvent();

    // Invoke from EnemyMovement (or any other killer) with the enemy's score value.
    public void KillEnemy(int scoreValue)
    {
        enemyKilled.Invoke(scoreValue);
    }
}
