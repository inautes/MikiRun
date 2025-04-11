using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public float initialGameSpeed = 5f;
    public float maxGameSpeed = 15f;
    public float gameSpeedIncreaseRate = 0.1f;
    public float distanceToChangeWorld = 1000f;
    
    [Header("World Themes")]
    public GameObject[] worldThemes;
    
    [Header("Player")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    
    [Header("UI References")]
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    
    private bool isGameActive = false;
    private bool isPaused = false;
    private float currentGameSpeed;
    private float distanceTraveled = 0f;
    private int currentWorldIndex = 0;
    private PlayerController player;
    
    private List<PlayerScore> highScores = new List<PlayerScore>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        LoadHighScores();
    }
    
    void Start()
    {
        currentGameSpeed = initialGameSpeed;
        
        if (player == null && playerPrefab != null && playerSpawnPoint != null)
        {
            GameObject playerObj = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
            player = playerObj.GetComponent<PlayerController>();
        }
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        StartGame();
    }
    
    void Update()
    {
        if (!isGameActive) return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        if (currentGameSpeed < maxGameSpeed)
        {
            currentGameSpeed += gameSpeedIncreaseRate * Time.deltaTime;
        }
        
        if (player != null)
        {
            distanceTraveled = player.GetDistanceTraveled();
            
            int newWorldIndex = Mathf.FloorToInt(distanceTraveled / distanceToChangeWorld) % worldThemes.Length;
            
            if (newWorldIndex != currentWorldIndex)
            {
                ChangeWorldTheme(newWorldIndex);
            }
        }
    }
    
    public void StartGame()
    {
        isGameActive = true;
        Time.timeScale = 1f;
    }
    
    public void GameOver()
    {
        isGameActive = false;
        
        SaveScore();
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null)
                pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }
    }
    
    private void ChangeWorldTheme(int newIndex)
    {
        if (worldThemes == null || worldThemes.Length == 0) return;
        
        if (currentWorldIndex < worldThemes.Length && worldThemes[currentWorldIndex] != null)
        {
            worldThemes[currentWorldIndex].SetActive(false);
        }
        
        if (newIndex < worldThemes.Length && worldThemes[newIndex] != null)
        {
            worldThemes[newIndex].SetActive(true);
        }
        
        currentWorldIndex = newIndex;
    }
    
    private void SaveScore()
    {
        string playerName = "Player"; // Could be customized in a real game
        
        PlayerScore newScore = new PlayerScore
        {
            playerName = playerName,
            distance = Mathf.RoundToInt(distanceTraveled)
        };
        
        highScores.Add(newScore);
        
        highScores.Sort((a, b) => b.distance.CompareTo(a.distance));
        
        if (highScores.Count > 10)
        {
            highScores.RemoveRange(10, highScores.Count - 10);
        }
        
        SaveHighScores();
    }
    
    private void SaveHighScores()
    {
        string highScoresJson = JsonUtility.ToJson(new HighScoreList { scores = highScores });
        
        PlayerPrefs.SetString("HighScores", highScoresJson);
        PlayerPrefs.Save();
    }
    
    private void LoadHighScores()
    {
        if (PlayerPrefs.HasKey("HighScores"))
        {
            string highScoresJson = PlayerPrefs.GetString("HighScores");
            HighScoreList loadedScores = JsonUtility.FromJson<HighScoreList>(highScoresJson);
            
            if (loadedScores != null && loadedScores.scores != null)
            {
                highScores = loadedScores.scores;
            }
        }
    }
    
    public List<PlayerScore> GetHighScores()
    {
        return highScores;
    }
    
    public float GetCurrentGameSpeed()
    {
        return currentGameSpeed;
    }
    
    public float GetDistanceTraveled()
    {
        return distanceTraveled;
    }
}

[System.Serializable]
public class PlayerScore
{
    public string playerName;
    public int distance;
}

[System.Serializable]
public class HighScoreList
{
    public List<PlayerScore> scores;
}
