using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Movement Settings")]
    // Velocidade de movimento entre os pontos
    public float moveSpeed = 2f;

    // Distância máxima que o alvo pode se afastar do centro em qualquer direção
    public float moveRange = 3f;

    // Intervalo de tempo entre escolher uma nova direção aleatória
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

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS — MOVIMENTO
    // -------------------------------------------------------------------------

    // Posição central — todas as direções são calculadas em torno dela
    private Vector3 centerPosition;

    // Próxima direção sorteada (Up, Down, Left, Right)
    private Vector3 currentDirection;

    // Contador para saber quando sortear uma nova direção
    private float directionTimer;

    private TargetSpawner spawner;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        centerPosition = transform.position;

        // Sorteia a primeira direção já no início
        PickRandomDirection();

        Debug.Log("[Target] Iniciado em: " + centerPosition + " | Direção inicial: " + currentDirection);
    }

    public void SetSpawner(TargetSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    void Update()
    {
        // Conta o tempo para trocar de direção periodicamente
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

    // Sorteia uma das 4 direções (cima, baixo, esquerda, direita)
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

        // Reseta o timer com uma pequena variação para parecer mais natural
        directionTimer = directionChangeInterval + Random.Range(-0.3f, 0.3f);

        Debug.Log("[Target] Nova direção sorteada: " + currentDirection +
                  " | Próxima troca em: " + directionTimer.ToString("F2") + "s");
    }

    private void MoveTarget()
    {
        // Calcula a posição que o movimento levaria
        Vector3 nextPosition = transform.position + currentDirection * moveSpeed * Time.deltaTime;

        // Calcula o offset dessa posição em relação ao centro
        Vector3 offsetFromCenter = nextPosition - centerPosition;

        // Se ultrapassar o range em X ou Y, inverte a direção correspondente
        // em vez de deixar o alvo fugir para fora da área permitida
        if (Mathf.Abs(offsetFromCenter.x) > moveRange)
        {
            currentDirection.x *= -1f;
            Debug.Log("[Target] Limite X atingido. Invertendo direção horizontal.");
        }

        if (Mathf.Abs(offsetFromCenter.y) > moveRange)
        {
            currentDirection.y *= -1f;
            Debug.Log("[Target] Limite Y atingido. Invertendo direção vertical.");
        }

        // Aplica o movimento já corrigido
        transform.Translate(currentDirection * moveSpeed * Time.deltaTime, Space.World);

        // Clampa a posição final para garantir que nunca ultrapasse o range,
        // mesmo em casos de delta time grande ou bordas múltiplas
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

        GameObject soundObj = new GameObject("TempHitSound");
        soundObj.transform.position = hitPoint;

        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = hitSoundClip;
        source.volume = hitSoundVolume;
        source.spatialBlend = 1f;

        if (randomizePitch)
            source.pitch = Random.Range(0.95f, 1.05f);

        source.Play();

        Debug.Log("[Target] Som de impacto tocado em: " + hitPoint +
                  " | Volume: " + hitSoundVolume);

        float clipDuration = hitSoundClip.length / source.pitch;
        Destroy(soundObj, clipDuration);
    }
}