using UnityEngine;

[CreateAssetMenu(menuName = "FSM/States/FireMario")]
public class FireMarioState : State
{
    public override void Tick(StateController controller)
    {
        // Body state logic here
    }
    public override void OnDamage(StateController controller)
    {
        controller.TransitionToBodyState(controller.superMarioState);
    }
}
