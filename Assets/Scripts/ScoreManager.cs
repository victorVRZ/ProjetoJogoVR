using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    private int score = 0;

    void Awake()
    {
        // Singleton — só existe um ScoreManager na cena
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();

        Debug.Log("[ScoreManager] +" + points + " pontos. Score total: " + score);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }

    public int GetScore() => score;
}