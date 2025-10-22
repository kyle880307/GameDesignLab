using UnityEngine;

[CreateAssetMenu(menuName = "FSM/States/DeadMario")]
public class DeadMarioState : State
{
    public override void Tick(StateController controller)
    {
        // Dead logic here (disable input, play animation, etc)
    }
}
