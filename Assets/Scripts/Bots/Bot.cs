using UnityEngine;

public class Bot : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // VARIÁVEIS PÚBLICAS
    // -------------------------------------------------------------------------

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

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        Debug.Log("[Bot] Bot iniciado: " + gameObject.name +
                  " | Intervalo: " + shootInterval + "s" +
                  " | Chance de acerto: " + hitChance + "%");

        if (targetSpawner == null)
        {
            Debug.LogError("[Bot] ERRO: targetSpawner não atribuído no Inspector! " +
                           "Arraste o BotTargetSpawner no campo 'Target Spawner'.");
        }

        // Inicializa o timer já no intervalo para atirar logo no início
        shootTimer = shootInterval;
    }

    void Update()
    {
        if (targetSpawner == null) return;

        // Conta o tempo até o próximo tiro
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            // Reseta o timer
            shootTimer = shootInterval;

            // Tenta atirar
            TryShoot();
        }
    }

    // -------------------------------------------------------------------------
    // MÉTODOS PRIVADOS
    // -------------------------------------------------------------------------

    void TryShoot()
    {
        Debug.Log("[Bot] Tentando atirar...");

        // Pega o alvo atual do spawner
        GameObject target = targetSpawner.GetCurrentTarget();

        if (target == null)
        {
            Debug.LogWarning("[Bot] AVISO: Nenhum alvo disponível no momento. Tiro cancelado.");
            return;
        }

        // Sorteia um número de 0 a 100 para determinar se acerta
        float roll = Random.Range(0f, 100f);

        Debug.Log("[Bot] Rolagem: " + roll.ToString("F1") + " | Necessário: abaixo de " + hitChance);

        if (roll <= hitChance)
        {
            // ACERTOU
            Debug.Log("[Bot] ACERTOU o alvo!");

            BotTarget botTarget = target.GetComponent<BotTarget>();

            if (botTarget == null)
            {
                Debug.LogError("[Bot] ERRO: O alvo não possui o script BotTarget!");
                return;
            }

            // Chama o método de acerto no alvo
            botTarget.GetHit();
        }
        else
        {
            // ERROU
            Debug.Log("[Bot] ERROU o alvo. (Rolagem " + roll.ToString("F1") +
                      " foi maior que " + hitChance + "%)");
        }
    }
}