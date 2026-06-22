using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject targetPrefab;
    public float respawnDelay = 1f;

    [Header("Target Especial")]
    public GameObject specialTargetPrefab;
    [Range(0f, 100f)]
    public float specialSpawnChance = 15f;

    [Header("Spawn Area")]
    public float spawnRangeX = 3f;
    public float spawnRangeY = 0f;
    public float spawnRangeZ = 0f;

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS
    // -------------------------------------------------------------------------

    // Controla se o spawner tem permissão para criar targets.
    // Começa como true por padrão para não quebrar cenas que não usam GameTimer/contagem.
    private bool spawningEnabled = true;

    void Start()
    {
        // Só spawna de imediato se já estiver habilitado.
        // Se o GameTimer chamar SetSpawningEnabled(false) antes deste Start
        // (ordem de execução pode variar), o spawn inicial é pulado e fica
        // aguardando a chamada de SetSpawningEnabled(true) mais adiante.
        if (spawningEnabled)
        {
            SpawnTarget();
        }
        else
        {
            Debug.Log("[TargetSpawner] Spawn inicial bloqueado — aguardando liberação externa.");
        }
    }

    // Chamado pelo GameTimer para liberar ou bloquear o spawn (ex: durante a contagem 3,2,1)
    public void SetSpawningEnabled(bool enabled)
    {
        bool wasDisabled = !spawningEnabled;
        spawningEnabled = enabled;

        Debug.Log("[TargetSpawner] Spawning " + (enabled ? "HABILITADO" : "DESABILITADO") +
                  " em: " + gameObject.name);

        // Se acabou de ser liberado e ainda não tem nenhum target ativo, spawna agora
        if (enabled && wasDisabled)
        {
            SpawnTarget();
        }
    }

    void SpawnTarget()
    {
        if (!spawningEnabled)
        {
            Debug.Log("[TargetSpawner] Tentativa de spawn bloqueada — spawning desabilitado.");
            return;
        }

        Vector3 spawnOffset = new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            Random.Range(-spawnRangeY, spawnRangeY),
            Random.Range(-spawnRangeZ, spawnRangeZ)
        );

        Vector3 spawnPosition = transform.position + spawnOffset;

        float roll = Random.Range(0f, 100f);
        bool spawnSpecial = (roll <= specialSpawnChance) && (specialTargetPrefab != null);

        GameObject prefabToSpawn = spawnSpecial ? specialTargetPrefab : targetPrefab;

        Debug.Log("[TargetSpawner] Rolagem: " + roll.ToString("F1") +
                  " | Resultado: " + (spawnSpecial ? "ESPECIAL" : "Normal"));

        if (prefabToSpawn == null)
        {
            Debug.LogError("[TargetSpawner] ERRO: Prefab a ser spawnado é null!");
            return;
        }

        GameObject newTarget = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        Target targetScript = newTarget.GetComponent<Target>();
        if (targetScript != null)
        {
            targetScript.SetSpawner(this);
        }
        else
        {
            Debug.LogError("[TargetSpawner] ERRO: O prefab spawnado não possui o script 'Target'!");
        }
    }

    public void OnTargetDestroyed()
    {
        if (!spawningEnabled)
        {
            Debug.Log("[TargetSpawner] Target destruído, mas spawning está desabilitado. " +
                      "Nenhum novo target será criado.");
            return;
        }

        Invoke(nameof(SpawnTarget), respawnDelay);
    }
}