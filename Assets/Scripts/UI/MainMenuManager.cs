using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button playButton;
    public Button highScoresButton;
    public Button quitButton;
    public GameObject highScorePanel;
    public Button closeHighScoreButton;
    
    [Header("High Score Display")]
    public Text[] highScoreTexts;
    
    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
        
        if (highScoresButton != null)
            highScoresButton.onClick.AddListener(OnHighScoresButtonClicked);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        
        if (closeHighScoreButton != null)
            closeHighScoreButton.onClick.AddListener(OnCloseHighScoreButtonClicked);
        
        if (highScorePanel != null)
            highScorePanel.SetActive(false);
        
        LoadHighScores();
    }
    
    void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("GameScene");
    }
    
    void OnHighScoresButtonClicked()
    {
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(true);
            LoadHighScores();
        }
    }
    
    void OnQuitButtonClicked()
    {
        Application.Quit();
    }
    
    void OnCloseHighScoreButtonClicked()
    {
        if (highScorePanel != null)
            highScorePanel.SetActive(false);
    }
    
    void LoadHighScores()
    {
        if (highScoreTexts == null || highScoreTexts.Length == 0) return;
        
        string highScoresJson = PlayerPrefs.GetString("HighScores", "");
        
        if (string.IsNullOrEmpty(highScoresJson))
        {
            for (int i = 0; i < highScoreTexts.Length; i++)
            {
                if (highScoreTexts[i] != null)
                {
                    highScoreTexts[i].text = (i + 1) + ". ---";
                }
            }
            return;
        }
        
        HighScoreList highScoreList = JsonUtility.FromJson<HighScoreList>(highScoresJson);
        
        if (highScoreList == null || highScoreList.scores == null)
        {
            return;
        }
        
        for (int i = 0; i < highScoreTexts.Length; i++)
        {
            if (highScoreTexts[i] != null)
            {
                if (i < highScoreList.scores.Count)
                {
                    PlayerScore score = highScoreList.scores[i];
                    highScoreTexts[i].text = (i + 1) + ". " + score.playerName + ": " + score.distance + "m";
                }
                else
                {
                    highScoreTexts[i].text = (i + 1) + ". ---";
                }
            }
        }
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
