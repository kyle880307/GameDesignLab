using UnityEngine;

[CreateAssetMenu(fileName = "MarioStats", menuName = "ScriptableObjects/MarioStats", order = 1)]
public class MarioStats : ScriptableObject
{
    public int maxHealth = 3;
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public bool hasBuff = false;
}
