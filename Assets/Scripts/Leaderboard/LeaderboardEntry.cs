using UnityEngine;

[System.Serializable]
public class LeaderboardEntry
{
    // Nome do participante (ex: "Player", "Bot 1")
    public string name;

    // Pontuação atual
    public int score;

    // True = player, False = bot
    public bool isPlayer;

    public LeaderboardEntry(string name, int score, bool isPlayer)
    {
        this.name = name;
        this.score = score;
        this.isPlayer = isPlayer;
    }
}