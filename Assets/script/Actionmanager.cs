using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public UnityEvent jump;
    public UnityEvent jumpHold;
    public UnityEvent<float> moveCheck; // float instead of int
    public UnityEvent attack;
    public UnityEvent dash;
    public UnityEvent drop;

    // =============================
    // Input Callbacks
    // =============================
    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (context.performed)
            jump.Invoke();
    }



    public void OnJumpHoldAction(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpHold.Invoke();
    }


    public void OnMoveAction(InputAction.CallbackContext context)
    {
        float move = context.ReadValue<float>();
        if (context.performed || context.canceled)
            moveCheck.Invoke(move);
    }

    public void OnAttackAction(InputAction.CallbackContext context)
    {
        if (context.performed)
            attack.Invoke();
    }

    public void OnDashAction(InputAction.CallbackContext context)
    {
        if (context.performed)
            dash.Invoke();
    }

    public void OnDropAction(InputAction.CallbackContext context)
    {
        if (context.performed)
            drop.Invoke();
    }
}
