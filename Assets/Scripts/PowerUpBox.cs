using UnityEngine;

public class PowerUpBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var mario = other.GetComponent<MarioStateController>();
        if (mario != null)
        {
            Debug.Log("Power-up collected!");
            mario.Powerup();
        }
    }
}