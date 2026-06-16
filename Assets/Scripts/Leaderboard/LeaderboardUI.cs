using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
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

    [Header("Bots")]
    public Bot[] bots;

    void Start()
    {
        Debug.Log("[LeaderboardUI] Iniciado.");

        // Garante que a leaderboard começa escondida
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
        else
            Debug.LogError("[LeaderboardUI] ERRO: leaderboardPanel não atribuído!");
       
    }

    // Chamado pelo GameTimer quando o tempo acaba
    public void ShowLeaderboard()
    {
        Debug.Log("[LeaderboardUI] Exibindo leaderboard...");

        // Sincroniza o score do player
        if (ScoreManager.Instance != null)
            LeaderboardManager.Instance.SetPlayerScore(ScoreManager.Instance.GetScore());

        // Pega o ranking e preenche os slots
        List<LeaderboardEntry> ranked = LeaderboardManager.Instance.GetRankedEntries();

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
        }

        // Exibe o painel
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        // Reinicia automaticamente após 5 segundos
        Debug.Log("[LeaderboardUI] Cena reiniciará em 5 segundos...");
        Invoke(nameof(RestartGame), 5f);
    }

    void RestartGame()
    {
        Debug.Log("[LeaderboardUI] Reiniciando cena...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}