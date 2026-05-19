using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    public Text scoreText;
    public int pointsPerMatch = 10;
    
    private int currentScore;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
        Debug.Log($"得分 +{points}！当前分数：{currentScore}");
    }

    public void AddMatchScore()
    {
        AddScore(pointsPerMatch);
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
        Debug.Log("分数已重置！");
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"分数：{currentScore}";
        }
    }
}
