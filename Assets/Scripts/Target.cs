using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveRange = 3f;
    public float directionChangeInterval = 1.5f;

    [Header("Score")]
    public int pointValue = 100;

    [Header("VFX")]
    public GameObject hitVFXPrefab;
    public float vfxDestroyDelay = 2f;

    [Header("Som de Impacto")]
    public AudioClip hitSoundClip;
    [Range(0f, 3f)]
    public float hitSoundVolume = 1.5f;
    public bool randomizePitch = true;

    [Header("Feedback de Pontuação (no Target)")]
    // Prefab com o componente FloatingScoreText (TextMeshPro 3D)
    public GameObject floatingScoreTextPrefab;

    // Altura do offset onde o texto aparece em relação ao ponto de impacto
    public float scoreTextHeightOffset = 0.2f;

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS — MOVIMENTO
    // -------------------------------------------------------------------------

    private Vector3 centerPosition;
    private Vector3 currentDirection;
    private float directionTimer;
    private TargetSpawner spawner;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        // Aplica os valores de dificuldade escolhidos no menu, se existirem.
        // Se o jogo for testado direto na cena (sem passar pelo MainMenu),
        // os valores configurados no Inspector permanecem inalterados.
        ApplyDifficultySettings();

        centerPosition = transform.position;
        PickRandomDirection();

        Debug.Log("[Target] Iniciado em: " + centerPosition + " | Direção inicial: " + currentDirection);
    }

    private void ApplyDifficultySettings()
    {
        if (DifficultyManager.Instance == null)
        {
            Debug.Log("[Target] DifficultyManager não encontrado. Usando valores do Inspector (modo teste direto).");
            return;
        }

        var settings = DifficultyManager.Instance.GetCurrentSettings();
        moveSpeed = settings.targetMoveSpeed;
        pointValue = settings.targetPointValue;

        Debug.Log("[Target] Dificuldade aplicada — Speed: " + moveSpeed + " | Points: " + pointValue);
    }

    public void SetSpawner(TargetSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    void Update()
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
        {
            PickRandomDirection();
        }

        MoveTarget();
    }

    // -------------------------------------------------------------------------
    // MOVIMENTO IMPREVISÍVEL
    // -------------------------------------------------------------------------

    private void PickRandomDirection()
    {
        int choice = Random.Range(0, 4);

        switch (choice)
        {
            case 0: currentDirection = Vector3.right; break;
            case 1: currentDirection = Vector3.left; break;
            case 2: currentDirection = Vector3.up; break;
            case 3: currentDirection = Vector3.down; break;
        }

        directionTimer = directionChangeInterval + Random.Range(-0.3f, 0.3f);

        Debug.Log("[Target] Nova direção sorteada: " + currentDirection +
                  " | Próxima troca em: " + directionTimer.ToString("F2") + "s");
    }

    private void MoveTarget()
    {
        Vector3 nextPosition = transform.position + currentDirection * moveSpeed * Time.deltaTime;
        Vector3 offsetFromCenter = nextPosition - centerPosition;

        if (Mathf.Abs(offsetFromCenter.x) > moveRange)
        {
            currentDirection.x *= -1f;
        }

        if (Mathf.Abs(offsetFromCenter.y) > moveRange)
        {
            currentDirection.y *= -1f;
        }

        transform.Translate(currentDirection * moveSpeed * Time.deltaTime, Space.World);

        Vector3 clampedOffset = transform.position - centerPosition;
        clampedOffset.x = Mathf.Clamp(clampedOffset.x, -moveRange, moveRange);
        clampedOffset.y = Mathf.Clamp(clampedOffset.y, -moveRange, moveRange);
        transform.position = centerPosition + clampedOffset;
    }

    // -------------------------------------------------------------------------
    // COLISÃO COM A FLECHA
    // -------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            Vector3 hitPoint = collision.contacts[0].point;

            SpawnHitVFX(hitPoint);
            PlayHitSound(hitPoint);
            SpawnFloatingScoreText(hitPoint);

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

        GameObject soundObj = new GameObject("TempHitSound");
        soundObj.transform.position = hitPoint;

        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = hitSoundClip;
        source.volume = hitSoundVolume;
        source.spatialBlend = 1f;

        if (randomizePitch)
            source.pitch = Random.Range(0.95f, 1.05f);

        source.Play();

        float clipDuration = hitSoundClip.length / source.pitch;
        Destroy(soundObj, clipDuration);
    }

    // -------------------------------------------------------------------------
    // TEXTO FLUTUANTE DE PONTUAÇÃO
    // -------------------------------------------------------------------------

    private void SpawnFloatingScoreText(Vector3 hitPoint)
    {
        if (floatingScoreTextPrefab == null)
        {
            Debug.LogWarning("[Target] floatingScoreTextPrefab não atribuído! " +
                             "Arraste o prefab com o script FloatingScoreText no Inspector.");
            return;
        }

        Vector3 spawnPos = hitPoint + Vector3.up * scoreTextHeightOffset;
        GameObject textObj = Instantiate(floatingScoreTextPrefab, spawnPos, Quaternion.identity);

        FloatingScoreText floatingScript = textObj.GetComponent<FloatingScoreText>();
        if (floatingScript != null)
        {
            floatingScript.Setup(pointValue);
        }
        else
        {
            Debug.LogError("[Target] ERRO: O prefab não possui o script FloatingScoreText!");
        }

        Debug.Log("[Target] Texto de pontuação spawnado em: " + spawnPos);
    }
}