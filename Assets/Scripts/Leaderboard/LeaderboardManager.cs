using UnityEngine;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    [Header("Configurações dos Participantes")]
    // Nome do player
    public string playerName = "Player";

    // Nomes dos bots (adiciona quantos bots tiver no jogo)
    public string[] botNames = { "Bot 1", "Bot 2", "Bot 3" };

    [Header("Pontos por Colocação")]
    // Pontos extras concedidos ao final por colocação
    public int firstPlaceBonus = 5000;
    public int secondPlaceBonus = 3500;
    public int thirdPlaceBonus = 2000;
    public int fourthPlaceBonus = 1000;

    // Lista interna de participantes
    private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Debug.Log("[LeaderboardManager] Iniciado.");
        InitializeEntries();
    }

    // Cria as entradas iniciais com score 0
    void InitializeEntries()
    {
        entries.Clear();

        // Adiciona o player
        entries.Add(new LeaderboardEntry(playerName, 0, true));
        Debug.Log("[LeaderboardManager] Player adicionado: " + playerName);

        // Adiciona os bots
        foreach (string botName in botNames)
        {
            entries.Add(new LeaderboardEntry(botName, 0, false));
            Debug.Log("[LeaderboardManager] Bot adicionado: " + botName);
        }
    }

    // Atualiza a pontuação do player
    public void SetPlayerScore(int score)
    {
        LeaderboardEntry player = entries.Find(e => e.isPlayer);
        if (player != null)
        {
            player.score = score;
            Debug.Log("[LeaderboardManager] Score do player atualizado: " + score);
        }
        else
        {
            Debug.LogWarning("[LeaderboardManager] AVISO: Player não encontrado na lista!");
        }
    }

    // Atualiza a pontuação de um bot pelo nome
    public void SetBotScore(string botName, int score)
    {
        LeaderboardEntry bot = entries.Find(e => e.name == botName && !e.isPlayer);
        if (bot != null)
        {
            bot.score = score;
            Debug.Log("[LeaderboardManager] Score do bot '" + botName + "' atualizado: " + score);
        }
        else
        {
            Debug.LogWarning("[LeaderboardManager] AVISO: Bot '" + botName + "' não encontrado!");
        }
    }

    // Retorna a lista ordenada por pontuação (maior primeiro)
    public List<LeaderboardEntry> GetRankedEntries()
    {
        List<LeaderboardEntry> ranked = new List<LeaderboardEntry>(entries);
        ranked.Sort((a, b) => b.score.CompareTo(a.score));

        Debug.Log("[LeaderboardManager] Ranking calculado:");
        for (int i = 0; i < ranked.Count; i++)
            Debug.Log("  " + (i + 1) + "º - " + ranked[i].name + ": " + ranked[i].score + " pts");

        return ranked;
    }

    // Retorna o bônus de pontos para cada colocação
    public int GetPlacementBonus(int placement)
    {
        switch (placement)
        {
            case 1: return firstPlaceBonus;
            case 2: return secondPlaceBonus;
            case 3: return thirdPlaceBonus;
            case 4: return fourthPlaceBonus;
            default: return 0;
        }
    }
}