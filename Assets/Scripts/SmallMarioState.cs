using UnityEngine;

[CreateAssetMenu(menuName = "FSM/States/SmallMario")]
public class SmallMarioState : State
{
    public override void Tick(StateController controller)
    {
        // Body state logic here (e.g., check for powerup/damage events)
    }
    public override void OnPowerup(StateController controller)
    {
        controller.TransitionToBodyState(controller.superMarioState);
    }
    public override void OnDamage(StateController controller)
    {
        controller.TransitionToBodyState(controller.deadMarioState);
    }
}
