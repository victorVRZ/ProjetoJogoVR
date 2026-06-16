using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Painel da Leaderboard")]
    // Painel raiz da leaderboard (começa desativado)
    public GameObject leaderboardPanel;

    [Header("Slots das Colocações (1º ao 4º)")]
    // Textos de cada colocação — arraste os 4 TextMeshPro aqui
    public TextMeshProUGUI[] placeNameTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] placeScoreTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] placeBonusTexts = new TextMeshProUGUI[4];

    [Header("Cores")]
    // Cor do nome quando é o player
    public Color playerColor = Color.yellow;

    // Cor do nome quando é um bot
    public Color botColor = Color.white;

    [Header("Countdown")]
    // Texto que exibe a contagem regressiva antes de reiniciar
    public TextMeshProUGUI countdownText;

    // Tempo em segundos antes de reiniciar
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
            Debug.LogWarning("[LeaderboardUI] AVISO: countdownText não atribuído. " +
                             "Arraste um TextMeshPro para o campo 'Countdown Text'.");
    }

    // -------------------------------------------------------------------------
    // EXIBIÇÃO DA LEADERBOARD
    // -------------------------------------------------------------------------

    // Chamado pelo GameTimer quando o tempo acaba
    public void ShowLeaderboard()
    {
        Debug.Log("[LeaderboardUI] Exibindo leaderboard...");

        // Para os bots
        foreach (Bot bot in bots)
        {
            if (bot != null)
                bot.StopBot();
        }

        // Sincroniza o score do player com o ScoreManager
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

        // Preenche os slots da UI
        for (int i = 0; i < 4; i++)
        {
            if (i >= ranked.Count)
            {
                if (placeNameTexts[i] != null) placeNameTexts[i].text = "-";
                if (placeScoreTexts[i] != null) placeScoreTexts[i].text = "-";
                if (placeBonusTexts[i] != null) placeBonusTexts[i].text = "";
                continue;
            }

            LeaderboardEntry entry = ranked[i];
            int placement = i + 1;
            int bonus = LeaderboardManager.Instance.GetPlacementBonus(placement);

            if (placeNameTexts[i] != null)
            {
                placeNameTexts[i].text = placement + "º  " + entry.name;
                placeNameTexts[i].color = entry.isPlayer ? playerColor : botColor;
            }

            if (placeScoreTexts[i] != null)
                placeScoreTexts[i].text = entry.score.ToString("N0") + " pts";

            if (placeBonusTexts[i] != null)
                placeBonusTexts[i].text = "+" + bonus.ToString("N0") + " bônus";

            Debug.Log("[LeaderboardUI] Slot " + placement + ": " + entry.name +
                      " | Score: " + entry.score + " | Bônus: " + bonus);
        }

        // Exibe o painel
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        // Inicia o countdown
        Debug.Log("[LeaderboardUI] Iniciando countdown de " + countdownDuration + " segundos...");
        StartCoroutine(CountdownRoutine());
    }

    // -------------------------------------------------------------------------
    // COUNTDOWN
    // -------------------------------------------------------------------------

    private IEnumerator CountdownRoutine()
    {
        // Ativa o texto de countdown
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        float remaining = countdownDuration;

        while (remaining > 0f)
        {
            // Atualiza o texto a cada frame
            int seconds = Mathf.CeilToInt(remaining);

            if (countdownText != null)
                countdownText.text = "Reiniciando em " + seconds + "...";

            Debug.Log("[LeaderboardUI] Countdown: " + seconds + "s restantes.");

            // Usa unscaledDeltaTime para funcionar mesmo com timeScale = 0
            remaining -= Time.unscaledDeltaTime;

            yield return null;
        }

        // Garante que mostra 0 antes de reiniciar
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