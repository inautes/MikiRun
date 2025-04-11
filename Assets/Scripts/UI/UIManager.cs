using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI healthText;
    public Image[] healthIcons;
    
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Pause Menu")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button quitButton;
    
    [Header("High Score UI")]
    public GameObject highScorePanel;
    public TextMeshProUGUI[] highScoreTexts;
    public Button closeHighScoreButton;
    
    private GameManager gameManager;
    private PlayerController player;
    
    void Start()
    {
        gameManager = GameManager.Instance;
        player = FindObjectOfType<PlayerController>();
        
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        
        if (closeHighScoreButton != null)
            closeHighScoreButton.onClick.AddListener(OnCloseHighScoreButtonClicked);
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        if (highScorePanel != null)
            highScorePanel.SetActive(false);
    }
    
    void Update()
    {
        if (gameManager == null || player == null) return;
        
        if (distanceText != null)
        {
            float distance = gameManager.GetDistanceTraveled();
            distanceText.text = "Distance: " + Mathf.RoundToInt(distance) + "m";
        }
        
        UpdateHealthDisplay();
    }
    
    void UpdateHealthDisplay()
    {
        if (player == null) return;
        
        int currentHealth = player.GetCurrentHealth();
        
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
        
        if (healthIcons != null)
        {
            for (int i = 0; i < healthIcons.Length; i++)
            {
                if (healthIcons[i] != null)
                {
                    healthIcons[i].enabled = i < currentHealth;
                }
            }
        }
    }
    
    public void ShowGameOver()
    {
        if (gameOverPanel == null) return;
        
        gameOverPanel.SetActive(true);
        
        if (finalScoreText != null && gameManager != null)
        {
            float distance = gameManager.GetDistanceTraveled();
            finalScoreText.text = "Distance: " + Mathf.RoundToInt(distance) + "m";
        }
    }
    
    public void ShowHighScores()
    {
        if (highScorePanel == null || gameManager == null) return;
        
        highScorePanel.SetActive(true);
        
        List<PlayerScore> highScores = gameManager.GetHighScores();
        
        if (highScoreTexts != null)
        {
            for (int i = 0; i < highScoreTexts.Length; i++)
            {
                if (highScoreTexts[i] != null)
                {
                    if (i < highScores.Count)
                    {
                        highScoreTexts[i].text = (i + 1) + ". " + highScores[i].playerName + ": " + highScores[i].distance + "m";
                    }
                    else
                    {
                        highScoreTexts[i].text = (i + 1) + ". ---";
                    }
                }
            }
        }
    }
    
    void OnRestartButtonClicked()
    {
        if (gameManager != null)
            gameManager.RestartGame();
    }
    
    void OnMainMenuButtonClicked()
    {
        if (gameManager != null)
            gameManager.ReturnToMainMenu();
    }
    
    void OnResumeButtonClicked()
    {
        if (gameManager != null)
            gameManager.TogglePause();
    }
    
    void OnQuitButtonClicked()
    {
        if (gameManager != null)
            gameManager.ReturnToMainMenu();
    }
    
    void OnCloseHighScoreButtonClicked()
    {
        if (highScorePanel != null)
            highScorePanel.SetActive(false);
    }
}
