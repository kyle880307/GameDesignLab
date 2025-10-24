using UnityEngine;

[CreateAssetMenu(menuName = "FSM/BuffStates/Invincible")]
public class InvincibleBuffState : BuffState
{
    public AudioClip invincibleAudio;
    public float flashSpeed = 10f;

    public override void Tick(BuffStateController controller)
    {
        // Make Mario flash/sparkle while invincible
        var sprite = controller.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            float alpha = Mathf.PingPong(Time.time * flashSpeed, 1f);
            Color c = sprite.color;
            c.a = Mathf.Lerp(0.5f, 1f, alpha);
            sprite.color = c;
        }
    }

    public override void OnEnter(BuffStateController controller)
    {
        if (invincibleAudio != null)
        {
            var audioSource = controller.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.clip = invincibleAudio;
                audioSource.Play();
            }
        }
    }

    public override void OnTimerEnd(BuffStateController controller)
    {
        controller.EndInvincibility();
    }

    public override void OnExit(BuffStateController controller)
    {
        // Reset sprite alpha
        var sprite = controller.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color c = sprite.color;
            c.a = 1f;
            sprite.color = c;
        }
    }
}