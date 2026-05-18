using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI pointsFeedbackText; // texto "+100" que aparece brevemente

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
        ShowFeedback(points);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }

    void ShowFeedback(int points)
    {
        if (pointsFeedbackText == null) return;

        pointsFeedbackText.text = "+" + points.ToString();
        pointsFeedbackText.gameObject.SetActive(true);

        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), 1f);
    }

    void HideFeedback()
    {
        if (pointsFeedbackText != null)
            pointsFeedbackText.gameObject.SetActive(false);
    }

    public int GetScore() => score;
}
