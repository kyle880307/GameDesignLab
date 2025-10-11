using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    private Vector3[] scoreTextPosition = {
        new Vector3(-596, 427, 0),
        new Vector3(0, 0, 0)
    };
    private Vector3[] restartButtonPosition = {
        new Vector3(844, 455, 0),
        new Vector3(0, -150, 0)
    };

    public GameObject scoreText;
    public Transform restartButton;
    public GameObject gameOverPanel;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            // subscribe to events
            gameManager.scoreChange.AddListener(SetScore);
            gameManager.gameOver.AddListener(GameOver);
            gameManager.gameStart.AddListener(GameStart);
            gameManager.gameRestart.AddListener(GameStart);
        }
        else
        {
            Debug.LogWarning("HUDManager: GameManager not found!");
        }
    }

    public void GameStart()
    {
        gameOverPanel.SetActive(false);
        scoreText.transform.localPosition = scoreTextPosition[0];
        restartButton.localPosition = restartButtonPosition[0];
    }

    public void SetScore(int score)
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        scoreText.transform.localPosition = scoreTextPosition[1];
        restartButton.localPosition = restartButtonPosition[1];
    }
}
