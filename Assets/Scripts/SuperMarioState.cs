using UnityEngine;

[CreateAssetMenu(menuName = "FSM/States/SuperMario")]
public class SuperMarioState : State
{
    public override void Tick(StateController controller)
    {
        // Body state logic here
    }
    public override void OnPowerup(StateController controller)
    {
        controller.TransitionToBodyState(controller.fireMarioState);
    }
    public override void OnDamage(StateController controller)
    {
        controller.TransitionToBodyState(controller.smallMarioState);
    }
}
