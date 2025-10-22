using UnityEngine;

public abstract class StateController : MonoBehaviour
{
    // Movement state (Idle, Running, etc.)
    public State currentMoveState;
    public State idleState;
    public State runningState;

    // Body state (Small, Super, Fire, Invincible, Dead, etc.)
    public State currentBodyState;
    public State smallMarioState;
    public State superMarioState;
    public State fireMarioState;
    public State invincibleSmallMarioState;
    public State deadMarioState;

    public float speed = 5f;

    public virtual void Update()
    {
        currentMoveState?.Tick(this);
        currentBodyState?.Tick(this);
    }

    public void TransitionToMoveState(State nextState)
    {
        currentMoveState = nextState;
        // nextState?.OnEnter(this);
    }

    public void TransitionToBodyState(State nextState)
    {
        currentBodyState = nextState;
        nextState?.OnEnter(this);
    }
}
