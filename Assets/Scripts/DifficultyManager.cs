using UnityEngine;

// Define os 3 níveis de dificuldade disponíveis
public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

// Singleton que persiste entre cenas — guarda a dificuldade escolhida no menu
// e fornece os valores de configuração para Target, Bot e BotTarget lerem.
public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Dificuldade Atual")]
    // Dificuldade selecionada no menu — padrão Medium caso o jogo seja
    // testado direto na cena de gameplay sem passar pelo menu
    public Difficulty currentDifficulty = Difficulty.Medium;

    [System.Serializable]
    public class DifficultySettings
    {
        [Header("Target")]
        public float targetMoveSpeed;
        public int targetPointValue;

        [Header("Bot")]
        [Tooltip("Chance do bot acertar o próprio alvo (0 a 100)")]
        public float botHitChance;

        [Tooltip("Intervalo entre tiros do bot — menor = bot mais rápido/agressivo")]
        public float botShootInterval;

        [Tooltip("Pontos que o bot ganha ao acertar seu próprio alvo")]
        public int botPointValue;
    }

    [Header("Configurações — Fácil")]
    public DifficultySettings easySettings = new DifficultySettings
    {
        targetMoveSpeed = 1.2f,
        targetPointValue = 100,
        botHitChance = 40f,
        botShootInterval = 3f,
        botPointValue = 50
    };

    [Header("Configurações — Médio")]
    public DifficultySettings mediumSettings = new DifficultySettings
    {
        targetMoveSpeed = 2f,
        targetPointValue = 150,
        botHitChance = 65f,
        botShootInterval = 2f,
        botPointValue = 100
    };

    [Header("Configurações — Difícil")]
    public DifficultySettings hardSettings = new DifficultySettings
    {
        targetMoveSpeed = 3.2f,
        targetPointValue = 250,
        botHitChance = 85f,
        botShootInterval = 1.2f,
        botPointValue = 200
    };

    void Awake()
    {
        // Garante que só existe uma instância, mesmo trocando de cena
        if (Instance != null && Instance != this)
        {
            Debug.Log("[DifficultyManager] Instância duplicada encontrada. Destruindo a nova.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[DifficultyManager] Iniciado e marcado como persistente entre cenas.");
    }

    // Chamado pelo MainMenuController quando o player escolhe a dificuldade
    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        Debug.Log("[DifficultyManager] Dificuldade definida: " + difficulty);
    }

    // Retorna as configurações da dificuldade atualmente selecionada
    public DifficultySettings GetCurrentSettings()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy: return easySettings;
            case Difficulty.Medium: return mediumSettings;
            case Difficulty.Hard: return hardSettings;
            default:
                Debug.LogWarning("[DifficultyManager] AVISO: Dificuldade não reconhecida. Usando Medium.");
                return mediumSettings;
        }
    }
}