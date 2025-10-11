using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI highscoreText;
    
    [Header("Game Data")]
    public IntVariable gameScore;

    void Start()
    {
        // Display the highscore when menu loads
        UpdateHighscoreDisplay();
    }

    // Call this function from the Start Game button
    public void StartGame()
    {
        SceneManager.LoadScene("Loading");
    }

    // Call this function from the Reset Highscore button
    public void ResetHighscore()
    {
        if (gameScore != null)
        {
            // Reset the highest value stored in the ScriptableObject
            gameScore.ResetHighestValue();
            gameScore.Value = 0;
            
            // Update the display
            UpdateHighscoreDisplay();
            
            Debug.Log("Highscore has been reset!");
        }
        else
        {
            Debug.LogWarning("GameScore ScriptableObject is not assigned!");
        }
    }

    // Helper function to update the highscore text display
    private void UpdateHighscoreDisplay()
    {
        if (highscoreText != null && gameScore != null)
        {
            highscoreText.GetComponent<TextMeshProUGUI>().text = "TOP- " + gameScore.previousHighestValue.ToString("D6");
        }
        else if (highscoreText == null)
        {
            Debug.LogWarning("Highscore Text is not assigned in the Inspector!");
        }
        else if (gameScore == null)
        {
            Debug.LogWarning("GameScore ScriptableObject is not assigned in the Inspector!");
        }
    }

    // Optional: Update display every frame in case you want real-time updates
    void Update()
    {
        // Uncomment if you want the highscore to update automatically
        // UpdateHighscoreDisplay();
    }
}
