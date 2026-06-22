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

    // Volume do som de impacto. PlayClipAtPoint trava em 1.0 — usamos um
    // AudioSource manual para poder ir além disso (ex: 1.5, 2.0 = mais alto que o normal)
    [Range(0f, 3f)]
    public float hitSoundVolume = 1.5f;

    // Pitch do som — variações leves (0.95 a 1.05) dão mais naturalidade quando vários alvos explodem
    public bool randomizePitch = true;

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

        // Cria um GameObject temporário com AudioSource manual.
        // Isso permite volume acima de 1.0, o que PlayClipAtPoint não permite.
        GameObject soundObj = new GameObject("TempHitSound");
        soundObj.transform.position = hitPoint;

        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = hitSoundClip;
        source.volume = hitSoundVolume; // pode passar de 1.0 aqui
        source.spatialBlend = 1f; // som 3D, igual ao PlayClipAtPoint padrão

        if (randomizePitch)
            source.pitch = Random.Range(0.95f, 1.05f);

        source.Play();

        Debug.Log("[Target] Som de impacto tocado em: " + hitPoint +
                  " | Volume: " + hitSoundVolume);

        // Destrói o objeto temporário após o clipe terminar (considerando o pitch)
        float clipDuration = hitSoundClip.length / source.pitch;
        Destroy(soundObj, clipDuration);
    }
}