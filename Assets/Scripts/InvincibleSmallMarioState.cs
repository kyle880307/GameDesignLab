using UnityEngine;

[CreateAssetMenu(menuName = "FSM/States/InvincibleSmallMario")]
public class InvincibleSmallMarioState : State
{
    public override void Tick(StateController controller)
    {
        // Body state logic here
    }
    public override void OnStarEnd(StateController controller)
    {
        controller.TransitionToBodyState(controller.smallMarioState);
    }
    public override void OnDamage(StateController controller)
    {
        // Ignore damage while invincible
    }
}
