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

    [Header("Som de Impacto")]
    // Clipe de som a ser tocado ao destruir o alvo
    public AudioClip hitSoundClip;

    // Volume do som de impacto (0 a 1)
    [Range(0f, 1f)]
    public float hitSoundVolume = 1f;

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
            Vector3 hitPoint = collision.contacts[0].point;

            // Spawna o VFX no ponto de impacto antes de destruir o alvo
            SpawnHitVFX(hitPoint);

            // Toca o som de impacto no ponto de impacto
            PlayHitSound(hitPoint);

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

        GameObject vfx = Instantiate(hitVFXPrefab, hitPoint, Quaternion.identity);

        Debug.Log("[Target] VFX spawnado em: " + hitPoint);

        Destroy(vfx, vfxDestroyDelay);
    }

    private void PlayHitSound(Vector3 hitPoint)
    {
        if (hitSoundClip == null)
        {
            Debug.LogWarning("[Target] Som de impacto não atribuído! " +
                             "Arraste um AudioClip no campo 'Hit Sound Clip'.");
            return;
        }

        // PlayClipAtPoint cria um GameObject temporário só para tocar o som,
        // o que funciona mesmo depois do Target original ser destruído.
        AudioSource.PlayClipAtPoint(hitSoundClip, hitPoint, hitSoundVolume);

        Debug.Log("[Target] Som de impacto tocado em: " + hitPoint);
    }
}