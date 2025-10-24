using UnityEngine;

[CreateAssetMenu(menuName = "FSM/BuffStates/Default")]
public class DefaultBuffState : BuffState
{
    public override void Tick(BuffStateController controller)
    {
        // No special behavior in default state
    }

    public override void OnEnter(BuffStateController controller)
    {
        // Return to normal appearance
        var sprite = controller.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = Color.white;
        }
    }
}