using UnityEngine;

[CreateAssetMenu(fileName = "GameConstants", menuName = "ScriptableObjects/GameConstants", order = 1)]
public class GameConstants : ScriptableObject
{
    [Header("Player Health")]
    public int maxLives = 3;

    [Header("Player Movement")]
    public float speed = 15f;
    public float maxSpeed = 60f;
    public float jumpForce = 15f;
    public float holdJumpForce = 50f;
    public float deathImpulse = 15f;

    [Header("Player Advanced Movement")]
    public float dashSpeed = 30f;
    public float dashTime = 0.1f;
    public float dashCooldown = 1f;
    public float dropSpeed = 40f;

    [Header("Player Starting Positions")]
    public Vector3 lab4StartingPosition = new Vector3(-40f, 3.5f, 0.0f);
    public Vector3 lab4_2StartingPosition = new Vector3(-44f, 3f, 0.0f);

    [Header("Enemy Movement")]
    public float enemyPatrolTime = 2.0f;
    public float enemyMaxOffset = 5.0f;
    public float enemyHitStunTime = 0.3f;

    [Header("Projectiles")]
    public float fireballSpeed = 10f;
    public float fireballMaxDistance = 15f;
    public int fireballDamage = 10;

    [Header("Special Skills")]
    public float specialSkillSpeed = 20f;
    public float specialSkillLifetime = 3f;
    public int specialSkillDamage = 100;

    [Header("Boss")]
    public int bossHealth = 100;
    public float bossSpeed = 2f;
    public int bossScoreValue = 10;

    [Header("Gameplay")]
    public int scoreLimitLab4 = 30;
    public int ScoreValue = 2;

    [Header("UI and Effects")]
    public float powerUpInteractionRange = 2f;
    public float boxBounceHeight = 1f;
    public float boxBounceDuration = 0.2f;

    [Header("Starman PowerUp")]
    public float starmanSpeed = 3f;
    public float starmanBounceForce = 8f;
    public float starmanLifetime = 10f;
    public int starmanScoreValue = 100;
}
