using UnityEngine;

public class BotTargetSpawner : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // VARIÁVEIS PÚBLICAS
    // -------------------------------------------------------------------------

    [Header("Configurações do Spawn")]
    // Prefab do alvo exclusivo dos bots
    public GameObject botTargetPrefab;

    // Tempo de espera antes de spawnar novo alvo após ser destruído
    public float respawnDelay = 1f;

    [Header("Área de Spawn")]
    // Range de posição aleatória em cada eixo
    public float spawnRangeX = 0f;
    public float spawnRangeY = 0f;
    public float spawnRangeZ = 0f;

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS
    // -------------------------------------------------------------------------

    // Referência ao alvo atualmente ativo
    private GameObject currentTarget;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        Debug.Log("[BotTargetSpawner] Iniciado em: " + gameObject.name);

        if (botTargetPrefab == null)
        {
            Debug.LogError("[BotTargetSpawner] ERRO: botTargetPrefab não atribuído no Inspector!");
            return;
        }

        SpawnTarget();
    }

    // -------------------------------------------------------------------------
    // MÉTODOS PÚBLICOS
    // -------------------------------------------------------------------------

    // Chamado pelo BotTarget ao ser destruído
    public void OnTargetDestroyed()
    {
        currentTarget = null;
        Debug.Log("[BotTargetSpawner] Alvo destruído. Respawnando em " + respawnDelay + "s...");
        Invoke(nameof(SpawnTarget), respawnDelay);
    }

    // Retorna o alvo atualmente ativo (usado pelo Bot para mirar)
    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }

    // -------------------------------------------------------------------------
    // MÉTODOS PRIVADOS
    // -------------------------------------------------------------------------

    void SpawnTarget()
    {
        if (botTargetPrefab == null)
        {
            Debug.LogError("[BotTargetSpawner] ERRO: botTargetPrefab é null no momento do spawn!");
            return;
        }

        if (currentTarget != null)
        {
            Debug.LogWarning("[BotTargetSpawner] AVISO: Já existe um alvo ativo. Spawn cancelado.");
            return;
        }

        // Calcula posição com offset aleatório
        Vector3 spawnOffset = new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            Random.Range(-spawnRangeY, spawnRangeY),
            Random.Range(-spawnRangeZ, spawnRangeZ)
        );

        Vector3 spawnPosition = transform.position + spawnOffset;

        currentTarget = Instantiate(botTargetPrefab, spawnPosition, Quaternion.identity);

        Debug.Log("[BotTargetSpawner] Alvo spawnado na posição: " + spawnPosition);

        // Passa referência do spawner para o alvo
        BotTarget botTargetScript = currentTarget.GetComponent<BotTarget>();

        if (botTargetScript == null)
        {
            Debug.LogError("[BotTargetSpawner] ERRO: Prefab não possui o script BotTarget!");
            return;
        }

        botTargetScript.SetSpawner(this);
        Debug.Log("[BotTargetSpawner] Spawn concluído com sucesso!");
    }
}