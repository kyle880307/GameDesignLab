using UnityEngine;

[CreateAssetMenu(menuName = "FSM/States/InvincibleSmallMario")]
public class InvincibleSmallMarioState : State
{
    private float invincibilityTimer = 0f;
    private const float INVINCIBILITY_DURATION = 2f;

    public override void Tick(StateController controller)
    {
        // Count down the invincibility timer
        invincibilityTimer -= Time.deltaTime;

        // Flash effect while invincible
        var sprite = controller.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            float alpha = Mathf.PingPong(Time.time * 10f, 1f);
            Color c = stateColor;
            c.a = Mathf.Lerp(0.3f, 1f, alpha);
            sprite.color = c;
        }

        // When timer expires, transition to the target state
        if (invincibilityTimer <= 0f)
        {
            // Reset sprite alpha
            if (sprite != null)
            {
                Color c = stateColor;
                c.a = 1f;
                sprite.color = c;
            }

            // Transition to the state we stored before entering invincibility
            if (controller.stateAfterInvincibility != null)
            {
                Debug.Log($"Invincibility ended, transitioning to {controller.stateAfterInvincibility.name}");
                controller.TransitionToBodyState(controller.stateAfterInvincibility);
                controller.stateAfterInvincibility = null; // Clear it
            }
            else
            {
                // Fallback to Small Mario
                Debug.Log("Invincibility ended, defaulting to Small Mario");
                controller.TransitionToBodyState(controller.smallMarioState);
            }
        }
    }

    public override void OnEnter(StateController controller)
    {
        base.OnEnter(controller);
        
        // Start the 2-second invincibility timer
        invincibilityTimer = INVINCIBILITY_DURATION;

        Debug.Log($"Entered invincibility state for {INVINCIBILITY_DURATION}s");
    }

    public override void OnPowerup(StateController controller)
    {
        // Can't collect powerups while in invincibility state
        Debug.Log("Can't use powerup during invincibility period");
    }

    public override void OnDamage(StateController controller)
    {
        // Ignore damage while invincible
        Debug.Log("Damage blocked - currently invincible!");
    }
}