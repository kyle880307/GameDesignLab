using UnityEngine;
using TMPro;

public class PowerUpBox : MonoBehaviour
{
    [Header("Game Constants")]
    public GameConstants gameConstants;

    [Header("Interaction Settings")]
    public string skillToUnlock = "SpecialAttack"; // Name of the skill to unlock
    public KeyCode interactKey = KeyCode.F;

    private float interactionRange;

    [Header("Visual Feedback")]
    public GameObject interactionPrompt; // UI element showing "Press F"
    public TextMeshProUGUI promptText;
    public SpriteRenderer boxSprite;
    public Color activeColor = Color.yellow;
    public Color usedColor = Color.gray;

    [Header("Effects")]
    public GameObject unlockEffect; // Particle effect when unlocked
    public AudioSource unlockSound;

    private bool isPlayerNearby = false;
    private bool isUsed = false;
    private PlayerMovement player;

    private void Start()
    {
        // Initialize values from GameConstants
        if (gameConstants != null)
        {
            interactionRange = gameConstants.powerUpInteractionRange;
        }
        else
        {
            Debug.LogWarning("GameConstants not assigned to PowerUpBox!");
            interactionRange = 2f; // Fallback value
        }

        // Initialize prompt
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (promptText != null)
            promptText.text = $"Press {interactKey} to unlock skill!";

        if (boxSprite != null)
            boxSprite.color = activeColor;
    }

    private void Update()
    {
        if (isUsed) return;

        CheckPlayerDistance();

        // Handle interaction input
        if (isPlayerNearby && Input.GetKeyDown(interactKey))
        {
            UnlockSkill();
        }
    }

    private void CheckPlayerDistance()
    {
        // Find player if not already found
        if (player == null)
        {
            player = PlayerMovement.instance;
            if (player == null) return;
        }

        // Check distance to player
        float distance = Vector2.Distance(transform.position, player.transform.position);
        bool wasNearby = isPlayerNearby;
        isPlayerNearby = distance <= interactionRange;

        // Show/hide prompt based on distance
        if (isPlayerNearby != wasNearby)
        {
            if (interactionPrompt != null)
                interactionPrompt.SetActive(isPlayerNearby);
        }
    }

    private void UnlockSkill()
    {
        if (player == null || isUsed) return;

        // Unlock the skill on the player
        player.UnlockSkill(skillToUnlock);

        // Mark as used
        isUsed = true;

        // Visual feedback
        if (boxSprite != null)
            boxSprite.color = usedColor;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        // Play effects
        if (unlockEffect != null)
        {
            GameObject effect = Instantiate(unlockEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        if (unlockSound != null)
            unlockSound.Play();

        Debug.Log($"Skill '{skillToUnlock}' unlocked!");
    }

    private void OnDrawGizmosSelected()
    {
        // Draw interaction range in editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    public void ResetBox()
    {
        isUsed = false;
        if (boxSprite != null)
            boxSprite.color = activeColor;
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
}
