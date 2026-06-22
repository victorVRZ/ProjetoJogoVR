using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject targetPrefab;
    public float respawnDelay = 1f;

    [Header("Target Especial")]
    // Prefab do target especial (vale mais pontos) — pode ser o mesmo modelo
    // com material/cor diferente, ou um prefab totalmente novo
    public GameObject specialTargetPrefab;

    // Chance de spawnar o target especial em vez do normal (0 a 100)
    [Range(0f, 100f)]
    public float specialSpawnChance = 15f;

    [Header("Spawn Area")]
    public float spawnRangeX = 3f;
    public float spawnRangeY = 0f;
    public float spawnRangeZ = 0f;

    void Start()
    {
        SpawnTarget();
    }

    void SpawnTarget()
    {
        // Posição aleatória dentro do range definido
        Vector3 spawnOffset = new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            Random.Range(-spawnRangeY, spawnRangeY),
            Random.Range(-spawnRangeZ, spawnRangeZ)
        );

        Vector3 spawnPosition = transform.position + spawnOffset;

        // Sorteia se este spawn será o target especial
        float roll = Random.Range(0f, 100f);
        bool spawnSpecial = (roll <= specialSpawnChance) && (specialTargetPrefab != null);

        GameObject prefabToSpawn = spawnSpecial ? specialTargetPrefab : targetPrefab;

        Debug.Log("[TargetSpawner] Rolagem: " + roll.ToString("F1") +
                  " | Chance especial: " + specialSpawnChance +
                  " | Resultado: " + (spawnSpecial ? "ESPECIAL" : "Normal"));

        if (prefabToSpawn == null)
        {
            Debug.LogError("[TargetSpawner] ERRO: Prefab a ser spawnado é null! " +
                           "Verifique se 'Target Prefab' está atribuído no Inspector.");
            return;
        }

        GameObject newTarget = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // Passa a referência do spawner para o target — funciona tanto para
        // o Target normal quanto para o especial, desde que ambos usem o mesmo script
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
        Invoke(nameof(SpawnTarget), respawnDelay);
    }
}