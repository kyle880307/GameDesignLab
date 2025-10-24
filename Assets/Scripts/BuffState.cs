using UnityEngine;

public abstract class BuffState : ScriptableObject
{
    public abstract void Tick(BuffStateController controller);

    public virtual void OnEnter(BuffStateController controller) { }
    public virtual void OnExit(BuffStateController controller) { }
    public virtual void OnTimerEnd(BuffStateController controller) { }
}