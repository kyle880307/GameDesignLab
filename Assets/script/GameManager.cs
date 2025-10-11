using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Game Constants")]
    public GameConstants gameConstants;

    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;
    public UnityEvent gamePause;
    public UnityEvent gameResume;
    public UnityEvent scoreLimitReached; // Event to show popup text
    public IntVariable gameScore;

    [Header("Score Limit Settings")]
    private int scoreLimitLab4; // Will be set from GameConstants
    private bool scoreLimitWarningShown = false;

    public override void Awake()
    {
        base.Awake();
        // Subscribe to scene change event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Destroy singletons when entering menu scene
        if (scene.name == "MainMenu")
        {
            DestroySingletons();
        }
        
        // Reset warning flag when changing scenes
        scoreLimitWarningShown = false;
    }

    void DestroySingletons()
    {
        // Destroy all persistent singletons
        if (GameManager.instance != null)
            Destroy(GameManager.instance.gameObject);
        
        if (HUDManager.instance != null)
            Destroy(HUDManager.instance.gameObject);
        
        if (PlayerMovement.instance != null)
            Destroy(PlayerMovement.instance.gameObject);
    }

    void Start()
    {
        // Initialize values from GameConstants
        if (gameConstants != null)
        {
            scoreLimitLab4 = gameConstants.scoreLimitLab4;
        }
        else
        {
            Debug.LogWarning("GameConstants not assigned to GameManager!");
            scoreLimitLab4 = 30; // Fallback value
        }

        gameStart.Invoke();
        gameScore.Value = 0;
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameRestart()
    {
        // reset score
        gameScore.Value = 0;
        scoreChange.Invoke(gameScore.Value);
        gameRestart.Invoke();
        Time.timeScale = 1.0f;
    }

    public void GamePause()
    {
        Time.timeScale = 0.0f;
        gamePause.Invoke();
    }
    public void GameResume()
    {
        Time.timeScale = 1.0f;
        gameResume.Invoke();
    }

    public void IncreaseScore(int increment)
    {
        // Check if we're in Lab4 scene and score limit is reached
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "Lab4" && gameScore.Value >= scoreLimitLab4)
        {
            // Don't increase score if limit reached
            if (!scoreLimitWarningShown)
            {
                scoreLimitReached.Invoke(); // Show popup text
                // scoreLimitWarningShown = true; // Only show once
                Debug.Log("Score limit reached in Lab4! Progress to next scene.");
            }
            return; // Exit without increasing score
        }

        // increase score by the increment amount
        gameScore.ApplyChange(increment);
        // invoke score change event with current score to update HUD
        scoreChange.Invoke(gameScore.Value);
    }

     public void SetScore(int score)
    {
        gameScore.Value = score;
        scoreChange.Invoke(gameScore.Value);
    }


    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void Menu()
    {
        Time.timeScale = 1.0f;
        // Reset game score before going to menu
        if (gameScore != null)
            gameScore.Value = 0;
        
        SceneManager.LoadScene("MainMenu");
    }
}