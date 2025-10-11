using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : Singleton<HUDManager>
{
    private Vector3[] scoreTextPosition = {
        new Vector3(-596, 423, 0),
        new Vector3(0, 0, 0)
    };
    private Vector3[] restartButtonPosition = {
        new Vector3(810, 450, 0),
        new Vector3(0, -150, 0)
    };
    private Vector3 pauseButtonPosition = new Vector3(100, 450, 0);
    private Vector3 resumeButtonPosition = new Vector3(100, 450, 0);

    public GameObject scoreText;
    public GameObject highscoreText;
    public Transform restartButton;
    public GameObject pauseButton;
    public GameObject resumeButton;
    public GameObject gameOverPanel;
    public IntVariable gameScore;
    // private GameManager gameManager;
    public GameObject menuButton;
    public GameObject GameOverText;
    
    [Header("Score Limit Warning")]
    public GameObject scoreLimitText; // Assign your popup text GameObject here

    void Start()
    {
        // gameManager = FindObjectOfType<GameManager>();
        scoreLimitText.SetActive(false);
    }

    public void GameStart()
    {
        scoreLimitText.SetActive(false);
        gameOverPanel.SetActive(false);
        scoreText.transform.localPosition = scoreTextPosition[0];
        restartButton.localPosition = restartButtonPosition[0];

        resumeButton.SetActive(false);
        pauseButton.SetActive(true);
        pauseButton.transform.localPosition = pauseButtonPosition;
        highscoreText.SetActive(false);
        menuButton.SetActive(false);
        GameOverText.SetActive(false);
    }

    public void SetScore(int score)
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
    }

    public void GameOver()
    {
        scoreLimitText.SetActive(false);
        gameOverPanel.SetActive(true);
        scoreText.transform.localPosition = scoreTextPosition[1];
        restartButton.localPosition = restartButtonPosition[1];
        pauseButton.SetActive(false);
        highscoreText.GetComponent<TextMeshProUGUI>().text = "TOP- " + gameScore.previousHighestValue.ToString("D6");
        highscoreText.SetActive(true);
        menuButton.SetActive(true);
        GameOverText.SetActive(true);
    }

    public void OnPauseUI()
    {
        scoreLimitText.SetActive(false);
        gameOverPanel.SetActive(true);
        pauseButton.SetActive(false);
        resumeButton.SetActive(true);
        resumeButton.transform.localPosition = resumeButtonPosition;
        highscoreText.GetComponent<TextMeshProUGUI>().text = "TOP- " + gameScore.previousHighestValue.ToString("D6");
        highscoreText.SetActive(true);
        menuButton.SetActive(true);
        GameOverText.SetActive(false);
    }

    public void OnResumeUI()
    {
        gameOverPanel.SetActive(false);
        resumeButton.SetActive(false);
        pauseButton.SetActive(true);
        pauseButton.transform.localPosition = pauseButtonPosition;
        highscoreText.SetActive(false);
        menuButton.SetActive(false);
        GameOverText.SetActive(false);
    }

    public void ShowScoreLimitWarning()
    {
            scoreLimitText.SetActive(true);
            StartCoroutine(HideScoreLimitWarningAfterDelay(3f));
    }

    private System.Collections.IEnumerator HideScoreLimitWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        scoreLimitText.SetActive(false);
    }

}
