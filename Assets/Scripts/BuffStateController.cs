using UnityEngine;

public class BuffStateController : MonoBehaviour
{
    public BuffState currentBuffState;
    public BuffState defaultBuffState;
    public BuffState invincibleBuffState;

    private float invincibilityTimer = 0f;
    private const float STAR_DURATION = 20f;

    public bool IsInvincible => currentBuffState == invincibleBuffState;

    public virtual void Update()
    {
        currentBuffState?.Tick(this);

        // Handle invincibility timer
        if (IsInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                currentBuffState?.OnTimerEnd(this);
            }
        }
    }

    public void TransitionToBuffState(BuffState nextState)
    {
        currentBuffState?.OnExit(this);
        currentBuffState = nextState;
        nextState?.OnEnter(this);
    }

    public void StartStarman()
    {
        invincibilityTimer = STAR_DURATION;
        invincibleBuffState?.OnEnter(this);
        TransitionToBuffState(invincibleBuffState);
    }

    public void EndInvincibility()
    {
        TransitionToBuffState(defaultBuffState);
    }

    public float GetInvincibilityTimer()
    {
        return invincibilityTimer;
    }
}