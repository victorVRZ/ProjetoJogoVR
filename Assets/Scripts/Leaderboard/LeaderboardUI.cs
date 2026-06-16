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

    [Header("Botão de Restart")]
    // Arraste o GameObject do botão físico 3D aqui
    public WorldSpaceRestartButton restartButton;

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

        if (LeaderboardManager.Instance == null)
        {
            Debug.LogError("[LeaderboardUI] ERRO: LeaderboardManager não encontrado na cena!");
            return;
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
                // Esconde slots sem participante
                if (placeNameTexts[i] != null) placeNameTexts[i].text = "-";
                if (placeScoreTexts[i] != null) placeScoreTexts[i].text = "-";
                if (placeBonusTexts[i] != null) placeBonusTexts[i].text = "";
                continue;
            }

            LeaderboardEntry entry = ranked[i];
            int placement = i + 1;
            int bonus = LeaderboardManager.Instance.GetPlacementBonus(placement);

            // Nome
            if (placeNameTexts[i] != null)
            {
                placeNameTexts[i].text = placement + "º  " + entry.name;
                placeNameTexts[i].color = entry.isPlayer ? playerColor : botColor;
            }

            // Pontuação base
            if (placeScoreTexts[i] != null)
                placeScoreTexts[i].text = entry.score.ToString("N0") + " pts";

            // Bônus de colocação
            if (placeBonusTexts[i] != null)
                placeBonusTexts[i].text = "+" + bonus.ToString("N0") + " bônus";

            Debug.Log("[LeaderboardUI] Slot " + placement + ": " + entry.name +
                      " | Score: " + entry.score + " | Bônus: " + bonus);
        }

        Debug.Log("[LeaderboardUI] Exibindo leaderboard...");


        // Exibe o painel
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        // Ativa o botão físico junto com a leaderboard
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);
        else
            Debug.LogWarning("[LeaderboardUI] AVISO: restartButton físico não atribuído!");

        foreach (Bot bot in bots)
        {
            if (bot != null)
                bot.StopBot();
        }
    }

    // Reinicia a cena ao clicar no botão
    void RestartGame()
    {
        Debug.Log("[LeaderboardUI] Reiniciando cena...");

        // Descongela o jogo antes de reiniciar
        Time.timeScale = 1f;

        Debug.Log("[LeaderboardUI] Reiniciando cena...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}