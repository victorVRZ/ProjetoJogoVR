using UnityEngine;

public class Bot : MonoBehaviour
{
    [Header("Configurações de Tiro")]
    public float shootInterval = 2f;

    [Range(0, 100)]
    public float hitChance = 75f;

    [Header("Referências")]
    public BotTargetSpawner targetSpawner;

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS
    // -------------------------------------------------------------------------

    private float shootTimer;
    private int totalBotScore = 0;

    // Controla se o bot está parado (usado tanto pela leaderboard quanto pela contagem inicial)
    private bool isStopped = false;

    void Start()
    {
        ApplyDifficultySettings();

        Debug.Log("[Bot] Bot iniciado: " + gameObject.name +
                  " | Intervalo: " + shootInterval + "s" +
                  " | Chance de acerto: " + hitChance + "%");

        if (targetSpawner == null)
            Debug.LogError("[Bot] ERRO: targetSpawner não atribuído no Inspector!");

        if (LeaderboardManager.Instance != null)
            LeaderboardManager.Instance.RegisterBot(this);
        else
            Debug.LogWarning("[Bot] AVISO: LeaderboardManager não encontrado!");

        shootTimer = shootInterval;
    }

    private void ApplyDifficultySettings()
    {
        if (DifficultyManager.Instance == null)
        {
            Debug.Log("[Bot] DifficultyManager não encontrado. Usando valores do Inspector (modo teste direto).");
            return;
        }

        var settings = DifficultyManager.Instance.GetCurrentSettings();
        hitChance = settings.botHitChance;
        shootInterval = settings.botShootInterval;

        Debug.Log("[Bot] Dificuldade aplicada — HitChance: " + hitChance + "% | ShootInterval: " + shootInterval + "s");
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

    // Chamado pelo LeaderboardUI ou pelo GameTimer (durante a contagem inicial)
    public void StopBot()
    {
        isStopped = true;
        Debug.Log("[Bot] Bot parado: " + gameObject.name);
    }

    // Chamado pelo GameTimer quando a contagem inicial termina
    public void ResumeBot()
    {
        isStopped = false;
        shootTimer = shootInterval; // reseta o timer para não atirar instantaneamente
        Debug.Log("[Bot] Bot retomado: " + gameObject.name);
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

            totalBotScore += botTarget.GetHit();

            Debug.Log("[Bot] Score acumulado: " + totalBotScore);

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