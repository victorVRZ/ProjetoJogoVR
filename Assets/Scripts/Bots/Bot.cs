using UnityEngine;

public class Bot : MonoBehaviour
{
    [Header("Configurações de Tiro")]
    // Intervalo em segundos entre cada tentativa de tiro
    public float shootInterval = 2f;

    // Chance de acertar o alvo (0 a 100)
    [Range(0, 100)]
    public float hitChance = 75f;

    [Header("Referências")]
    // Spawner do alvo exclusivo deste bot
    public BotTargetSpawner targetSpawner;

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS
    // -------------------------------------------------------------------------

    // Contador interno para controlar o intervalo de tiro
    private float shootTimer;

    // Acumula a pontuação total do bot durante a partida
    private int totalBotScore = 0;

    // Controla se o bot está parado
    private bool isStopped = false;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        Debug.Log("[Bot] Bot iniciado: " + gameObject.name +
                  " | Intervalo: " + shootInterval + "s" +
                  " | Chance de acerto: " + hitChance + "%");

        if (targetSpawner == null)
            Debug.LogError("[Bot] ERRO: targetSpawner não atribuído no Inspector!");

        // Registra este bot no LeaderboardManager automaticamente
        if (LeaderboardManager.Instance != null)
            LeaderboardManager.Instance.RegisterBot(this);
        else
            Debug.LogWarning("[Bot] AVISO: LeaderboardManager não encontrado! " +
                             "O bot não será registrado na leaderboard.");

        shootTimer = shootInterval;
    }

    void Update()
    {
        if (targetSpawner == null || isStopped) return;

        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            shootTimer = shootInterval;
            TryShoot();
        }
    }

    // -------------------------------------------------------------------------
    // MÉTODOS PÚBLICOS
    // -------------------------------------------------------------------------

    // Chamado pelo LeaderboardUI para parar o bot
    public void StopBot()
    {
        isStopped = true;
        Debug.Log("[Bot] Bot parado: " + gameObject.name);
    }

    // -------------------------------------------------------------------------
    // MÉTODOS PRIVADOS
    // -------------------------------------------------------------------------

    void TryShoot()
    {
        Debug.Log("[Bot] Tentando atirar...");

        GameObject target = targetSpawner.GetCurrentTarget();

        if (target == null)
        {
            Debug.LogWarning("[Bot] AVISO: Nenhum alvo disponível. Tiro cancelado.");
            return;
        }

        float roll = Random.Range(0f, 100f);
        Debug.Log("[Bot] Rolagem: " + roll.ToString("F1") + " | Necessário: abaixo de " + hitChance);

        if (roll <= hitChance)
        {
            Debug.Log("[Bot] ACERTOU o alvo!");

            BotTarget botTarget = target.GetComponent<BotTarget>();

            if (botTarget == null)
            {
                Debug.LogError("[Bot] ERRO: O alvo não possui o script BotTarget!");
                return;
            }

            // Soma os pontos retornados pelo GetHit()
            totalBotScore += botTarget.GetHit();

            Debug.Log("[Bot] Score acumulado: " + totalBotScore);

            // Atualiza passando a própria referência — sem depender de nome
            if (LeaderboardManager.Instance != null)
                LeaderboardManager.Instance.SetBotScore(this, totalBotScore);
            else
                Debug.LogWarning("[Bot] AVISO: LeaderboardManager não encontrado!");
        }
        else
        {
            Debug.Log("[Bot] ERROU o alvo. (Rolagem " + roll.ToString("F1") +
                      " foi maior que " + hitChance + "%)");
        }
    }
}