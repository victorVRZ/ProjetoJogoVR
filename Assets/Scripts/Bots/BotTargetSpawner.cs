using UnityEngine;

public class BotTargetSpawner : MonoBehaviour
{
    [Header("Configurações do Spawn")]
    public GameObject botTargetPrefab;
    public float respawnDelay = 1f;

    [Header("Área de Spawn")]
    public float spawnRangeX = 0f;
    public float spawnRangeY = 0f;
    public float spawnRangeZ = 0f;

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS
    // -------------------------------------------------------------------------

    private GameObject currentTarget;

    // Controla se o spawner tem permissão para criar targets dos bots.
    // Começa true por padrão para não quebrar cenas sem GameTimer/contagem.
    private bool spawningEnabled = true;

    void Start()
    {
        Debug.Log("[BotTargetSpawner] Iniciado em: " + gameObject.name);

        if (botTargetPrefab == null)
        {
            Debug.LogError("[BotTargetSpawner] ERRO: botTargetPrefab não atribuído no Inspector!");
            return;
        }

        if (spawningEnabled)
        {
            SpawnTarget();
        }
        else
        {
            Debug.Log("[BotTargetSpawner] Spawn inicial bloqueado — aguardando liberação externa.");
        }
    }

    // Chamado pelo GameTimer para liberar ou bloquear o spawn dos alvos dos bots
    public void SetSpawningEnabled(bool enabled)
    {
        bool wasDisabled = !spawningEnabled;
        spawningEnabled = enabled;

        Debug.Log("[BotTargetSpawner] Spawning " + (enabled ? "HABILITADO" : "DESABILITADO") +
                  " em: " + gameObject.name);

        if (enabled && wasDisabled && currentTarget == null)
        {
            SpawnTarget();
        }
    }

    public void OnTargetDestroyed()
    {
        currentTarget = null;

        if (!spawningEnabled)
        {
            Debug.Log("[BotTargetSpawner] Alvo destruído, mas spawning desabilitado. Nenhum novo será criado.");
            return;
        }

        Debug.Log("[BotTargetSpawner] Alvo destruído. Respawnando em " + respawnDelay + "s...");
        Invoke(nameof(SpawnTarget), respawnDelay);
    }

    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }

    private void SpawnTarget()
    {
        if (!spawningEnabled)
        {
            Debug.Log("[BotTargetSpawner] Tentativa de spawn bloqueada — spawning desabilitado.");
            return;
        }

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

        Vector3 spawnOffset = new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            Random.Range(-spawnRangeY, spawnRangeY),
            Random.Range(-spawnRangeZ, spawnRangeZ)
        );

        Vector3 spawnPosition = transform.position + spawnOffset;

        currentTarget = Instantiate(botTargetPrefab, spawnPosition, Quaternion.identity);

        Debug.Log("[BotTargetSpawner] Alvo spawnado na posição: " + spawnPosition);

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