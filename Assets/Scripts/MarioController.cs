using UnityEngine;

public class MarioStateController : StateController
{
    // Static field to persist body state across scenes
    public static State savedBodyState;
    public static State savedStateAfterInvincibility;

    private void OnEnable()
    {
        // Restore saved state when scene loads or Mario respawns
        if (savedBodyState != null)
        {
            currentBodyState = savedBodyState;
            currentBodyState?.OnEnter(this);
        }
        
        // Restore the invincibility target state if it was saved
        if (savedStateAfterInvincibility != null)
        {
            stateAfterInvincibility = savedStateAfterInvincibility;
        }
    }
     private void OnDisable()
    {
        // Save state when scene unloads or Mario is destroyed
        savedBodyState = currentBodyState;
        savedStateAfterInvincibility = stateAfterInvincibility;
    }
    protected override void Start()
    {
        base.Start();
        
        // Initialize to Small Mario state if not set and no saved state
        if (currentBodyState == null && savedBodyState == null)
        {
            currentBodyState = smallMarioState;
            currentBodyState?.OnEnter(this);
        }
        
        if (currentMoveState == null)
        {
            currentMoveState = idleState;
        }
    }

    public void Powerup()
    {
        OnPowerup();
    }

    public void Damage()
    {
        OnDamage();
    }

    public void Star()
    {
        OnStarman();
    }

}

