using UnityEngine;

public class MarioStateController : StateController
{
    // Static field to persist body state across scenes
    public static State savedBodyState;

    private void OnEnable()
    {
        if (savedBodyState != null)
            currentBodyState = savedBodyState;
            currentBodyState.OnEnter(this); // <-- Add this line
    }

    private void OnDisable()
    {
        savedBodyState = currentBodyState;
    }

    public void Powerup() => currentBodyState?.OnPowerup(this);
    public void Damage() => currentBodyState?.OnDamage(this);
    public void Star() => currentBodyState?.OnStar(this);
    public void StarEnd() => currentBodyState?.OnStarEnd(this);
}

