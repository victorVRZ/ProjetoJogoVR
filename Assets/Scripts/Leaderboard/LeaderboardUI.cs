using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Painel da Leaderboard")]
    public GameObject leaderboardPanel;

    [Header("Slots das Colocações (1º ao 4º)")]
    public TextMeshProUGUI[] placeNameTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] placeScoreTexts = new TextMeshProUGUI[4];

    [Header("Cores")]
    public Color playerColor = Color.yellow;
    public Color botColor = Color.white;

    [Header("Countdown")]
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 5f;

    [Header("Bots")]
    public Bot[] bots;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        Debug.Log("[LeaderboardUI] Iniciado.");

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
        else
            Debug.LogError("[LeaderboardUI] ERRO: leaderboardPanel não atribuído!");

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
        else
            Debug.LogWarning("[LeaderboardUI] AVISO: countdownText não atribuído.");
    }

    // -------------------------------------------------------------------------
    // EXIBIÇÃO DA LEADERBOARD
    // -------------------------------------------------------------------------

    public void ShowLeaderboard()
    {
        Debug.Log("[LeaderboardUI] Exibindo leaderboard...");

        // Para os bots
        foreach (Bot bot in bots)
        {
            if (bot != null)
                bot.StopBot();
        }

        // Sincroniza o score do player
        if (ScoreManager.Instance != null)
        {
            LeaderboardManager.Instance.SetPlayerScore(ScoreManager.Instance.GetScore());
            Debug.Log("[LeaderboardUI] Score do player sincronizado: " + ScoreManager.Instance.GetScore());
        }
        else
        {
            Debug.LogWarning("[LeaderboardUI] AVISO: ScoreManager não encontrado!");
        }

        // Pega o ranking ordenado
        List<LeaderboardEntry> ranked = LeaderboardManager.Instance.GetRankedEntries();

        // Preenche os slots
        for (int i = 0; i < 4; i++)
        {
            if (i >= ranked.Count)
            {
                if (placeNameTexts[i] != null) placeNameTexts[i].text = "-";
                if (placeScoreTexts[i] != null) placeScoreTexts[i].text = "-";
                continue;
            }

            LeaderboardEntry entry = ranked[i];
            int placement = i + 1;

            if (placeNameTexts[i] != null)
            {
                placeNameTexts[i].text = placement + "º  " + entry.name;
                placeNameTexts[i].color = entry.isPlayer ? playerColor : botColor;
            }

            if (placeScoreTexts[i] != null)
                placeScoreTexts[i].text = entry.score.ToString("N0") + " pts";

            Debug.Log("[LeaderboardUI] Slot " + placement + ": " + entry.name +
                      " | Score: " + entry.score);
        }

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        Debug.Log("[LeaderboardUI] Iniciando countdown de " + countdownDuration + " segundos...");
        StartCoroutine(CountdownRoutine());
    }

    // -------------------------------------------------------------------------
    // COUNTDOWN
    // -------------------------------------------------------------------------

    private IEnumerator CountdownRoutine()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        float remaining = countdownDuration;

        while (remaining > 0f)
        {
            int seconds = Mathf.CeilToInt(remaining);

            if (countdownText != null)
                countdownText.text = "Reiniciando " + seconds + "...";

            Debug.Log("[LeaderboardUI] Countdown: " + seconds + "s restantes.");

            remaining -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (countdownText != null)
            countdownText.text = "Reiniciando...";

        RestartGame();
    }

    // -------------------------------------------------------------------------
    // RESTART
    // -------------------------------------------------------------------------

    void RestartGame()
    {
        Debug.Log("[LeaderboardUI] Reiniciando cena...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}