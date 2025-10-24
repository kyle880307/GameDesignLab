using UnityEngine;

public abstract class State : ScriptableObject
{
    public AudioClip stateAudio;
    public Color stateColor;

    public abstract void Tick(StateController controller);

    public virtual void OnPowerup(StateController controller) { }
    public virtual void OnDamage(StateController controller) { }
    public virtual void OnExit(StateController controller) { }
    public virtual void OnEnter(StateController controller)
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
}