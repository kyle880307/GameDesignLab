using UnityEngine;

public class StarmanBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var mario = other.GetComponent<MarioStateController>();
        if (mario != null)
        {
            mario.OnStarman();
        }
    }
}