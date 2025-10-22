using UnityEngine;

[CreateAssetMenu(menuName = "States/IdleState")]
public class IdleState : State
{
    public override void Tick(StateController controller)
    {
        var horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontal) > 0.01f)
        {
            controller.TransitionToMoveState(controller.runningState);
        }
    }
}
