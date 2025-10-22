using UnityEngine;

public class DamageBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var mario = other.GetComponent<MarioStateController>();
        if (mario != null)
        {
            mario.Damage();
        }
    }
}