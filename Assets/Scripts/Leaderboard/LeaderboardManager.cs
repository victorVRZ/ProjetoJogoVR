using UnityEngine;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    [Header("Configurações do Player")]
    public string playerName = "Player";

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

        entries.Clear();
        entries.Add(new LeaderboardEntry(playerName, 0, true));
        Debug.Log("[LeaderboardManager] Player adicionado: " + playerName);
    }

    // Chamado pelo Bot no Start() para se registrar automaticamente
    public void RegisterBot(Bot bot)
    {
        if (bot == null)
        {
            Debug.LogError("[LeaderboardManager] ERRO: RegisterBot recebeu referência null!");
            return;
        }

        LeaderboardEntry existing = entries.Find(e => e.name == bot.gameObject.name && !e.isPlayer);
        if (existing != null)
        {
            Debug.LogWarning("[LeaderboardManager] AVISO: Bot '" + bot.gameObject.name + "' já registrado. Ignorando.");
            return;
        }

        entries.Add(new LeaderboardEntry(bot.gameObject.name, 0, false));
        Debug.Log("[LeaderboardManager] Bot registrado: " + bot.gameObject.name);
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
            Debug.LogWarning("[LeaderboardManager] AVISO: Player não encontrado!");
        }
    }

    // Atualiza a pontuação de um bot pela referência direta
    public void SetBotScore(Bot bot, int score)
    {
        if (bot == null)
        {
            Debug.LogError("[LeaderboardManager] ERRO: SetBotScore recebeu referência null!");
            return;
        }

        LeaderboardEntry entry = entries.Find(e => e.name == bot.gameObject.name && !e.isPlayer);
        if (entry != null)
        {
            entry.score = score;
            Debug.Log("[LeaderboardManager] Score do bot '" + bot.gameObject.name + "' atualizado: " + score);
        }
        else
        {
            Debug.LogWarning("[LeaderboardManager] AVISO: Bot '" + bot.gameObject.name + "' não encontrado!");
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
}