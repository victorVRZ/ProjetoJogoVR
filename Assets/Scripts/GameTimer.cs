using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Configurações do Timer")]
    public float totalTime = 60f;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("Leaderboard")]
    // Arraste o LeaderboardUI aqui
    public LeaderboardUI leaderboardUI;

    private float currentTime;
    private bool isRunning = true;

    void Start()
    {
        Debug.Log("[GameTimer] Timer iniciado com " + totalTime + " segundos.");

        if (timerText == null)
            Debug.LogError("[GameTimer] ERRO: timerText não atribuído!");

        if (leaderboardUI == null)
            Debug.LogError("[GameTimer] ERRO: leaderboardUI não atribuído! " +
                           "Arraste o LeaderboardUI no campo correspondente.");

        currentTime = totalTime;
        UpdateTimerUI();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            UpdateTimerUI();
            isRunning = false;
            Debug.Log("[GameTimer] Tempo esgotado!");
            TimeUp();
            return;
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerText.color = currentTime <= 10f ? Color.red : Color.white;
    }

    void TimeUp()
    {
        // Exibe a leaderboard ao invés de reiniciar direto
        if (leaderboardUI != null)
            leaderboardUI.ShowLeaderboard();
        else
        {
            Debug.LogWarning("[GameTimer] LeaderboardUI não encontrado. Reiniciando diretamente.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void SetRunning(bool running)
    {
        isRunning = running;
        Debug.Log("[GameTimer] Timer " + (running ? "retomado." : "pausado."));
    }

    public float GetCurrentTime() => currentTime;
}