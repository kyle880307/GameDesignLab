using UnityEngine;

[CreateAssetMenu(menuName = "FSM/States/Running")]
public class RunningState : State
{
    public override void Tick(StateController controller)
    {
        var horizontal = Input.GetAxis("Horizontal");
        var rb = controller.GetComponent<Rigidbody2D>();
        var speed = controller.speed;

        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        // Flip sprite if needed
        var sprite = controller.GetComponent<SpriteRenderer>();
        if (sprite != null && horizontal != 0)
            sprite.flipX = horizontal < 0;

        // Transition to Idle if not moving
        if (Mathf.Abs(horizontal) < 0.01f)
        {
            controller.TransitionToMoveState(controller.idleState);
        }
    }
}
