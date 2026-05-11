using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject targetPrefab;
    public float respawnDelay = 1f;

    [Header("Spawn Area")]
    public float spawnRangeX = 3f;
    public float spawnRangeY = 0f;
    public float spawnRangeZ = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnTarget();
    }

    // Update is called once per frame
    void Update()
    {
        
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

        GameObject newTarget = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);

        // Passa a referência do spawner para o target
        Target targetScript = newTarget.GetComponent<Target>();
        if (targetScript != null)
            targetScript.SetSpawner(this);
    }
    public void OnTargetDestroyed()
    {
        Invoke(nameof(SpawnTarget), respawnDelay);
    }
}
