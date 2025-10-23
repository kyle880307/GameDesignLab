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
    // Track which state to return to after invincibility
    public State stateAfterInvincibility;

    // Reference to buff controller (for Starman invincibility)
    private BuffStateController buffController;

    public bool IsInInvincibleState => currentBodyState == invincibleSmallMarioState;
    public bool IsStarInvincible => buffController != null && buffController.IsInvincible;
    public float speed = 5f;
    protected virtual void Start()
    {
        buffController = GetComponent<BuffStateController>();
        if (buffController == null)
        {
            Debug.LogWarning("No BuffStateController found on " + gameObject.name);
        }
    }


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
        currentBodyState?.OnExit(this);
        currentBodyState = nextState;
        nextState?.OnEnter(this);
    }
    // Called when Mario gets a powerup (Mushroom/FireFlower)
    public void OnPowerup()
    {
        // If star invincible or in damage invincibility, might want to ignore
        if (IsStarInvincible)
            return;

        currentBodyState?.OnPowerup(this);
    }
    
        // Called when Mario takes damage
    public void OnDamage()
    {
        // If already invincible (star or damage invincibility), ignore damage
        if (IsStarInvincible || IsInInvincibleState)
        {
            Debug.Log("Damage blocked - invincible!");
            return;
        }

        // Store which state to return to after invincibility wears off
        if (currentBodyState == superMarioState)
        {
            stateAfterInvincibility = smallMarioState;
        }
        else if (currentBodyState == fireMarioState)
        {
            stateAfterInvincibility = superMarioState;
        }

        // Transition based on current state
        currentBodyState?.OnDamage(this);
    }

    // Called when Mario collects a star
    public void OnStarman()
    {
        if (buffController != null)
        {
            buffController.StartStarman();
        }
    }
}
