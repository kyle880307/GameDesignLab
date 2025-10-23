using UnityEngine;

public abstract class BuffState : ScriptableObject
{
    public abstract void Tick(BuffStateController controller);

    public virtual void OnEnter(BuffStateController controller)
    {
        if (stateAudio != null)
        {
            var audioSource = controller.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.clip = stateAudio;
                audioSource.Play();
            }
        }
        
        var sprite = controller.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = stateColor;
        }
    }
    public virtual void OnExit(BuffStateController controller) { }
    public virtual void OnTimerEnd(BuffStateController controller) { }
}