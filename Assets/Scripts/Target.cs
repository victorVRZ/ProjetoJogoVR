using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveRange = 3f;

    [Header("Score")]
    public int pointValue = 100;

    [Header("VFX")]
    // Arraste aqui um dos prefabs do asset VFX Impact and Hit
    public GameObject hitVFXPrefab;

    // Tempo até o VFX ser destruído automaticamente
    public float vfxDestroyDelay = 2f;

    private Vector3 startPosition;
    private float direction = 1f;
    private TargetSpawner spawner;

    void Start()
    {
        startPosition = transform.position;
    }

    public void SetSpawner(TargetSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    void Update()
    {
        float currentOffset = transform.position.x - startPosition.x;

        if (currentOffset >= moveRange)
            direction = -1f;
        else if (currentOffset <= -moveRange)
            direction = 1f;

        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            // Spawna o VFX no ponto de impacto antes de destruir o alvo
            SpawnHitVFX(collision.contacts[0].point);

            // Adiciona pontos ao acertar
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(pointValue);

            Destroy(collision.gameObject);
            Destroy(gameObject);

            if (spawner != null)
                spawner.OnTargetDestroyed();
        }
    }

    private void SpawnHitVFX(Vector3 hitPoint)
    {
        if (hitVFXPrefab == null)
        {
            Debug.LogWarning("[Target] VFX não atribuído! Arraste um prefab do " +
                             "VFX Impact and Hit no campo 'Hit VFX Prefab'.");
            return;
        }

        // Instancia o VFX no ponto exato de contato entre a flecha e o alvo
        GameObject vfx = Instantiate(hitVFXPrefab, hitPoint, Quaternion.identity);

        Debug.Log("[Target] VFX spawnado em: " + hitPoint);

        // Destrói o VFX após o delay para não acumular partículas na cena
        Destroy(vfx, vfxDestroyDelay);
    }
}