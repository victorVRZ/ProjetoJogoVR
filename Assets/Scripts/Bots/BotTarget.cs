using UnityEngine;

public class BotTarget : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // VARIÁVEIS PÚBLICAS
    // -------------------------------------------------------------------------

    [Header("Movimento")]
    public float moveSpeed = 2f;
    public float moveRange = 3f;

    [Header("Score")]
    // Pontos que o bot ganha ao acertar este alvo.
    // Sobrescrito automaticamente pela dificuldade selecionada no menu, se houver.
    public int pointValue = 100;

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS
    // -------------------------------------------------------------------------

    private Vector3 startPosition;
    private float direction = 1f;
    private BotTargetSpawner spawner;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        ApplyDifficultySettings();

        startPosition = transform.position;
        Debug.Log("[BotTarget] Alvo criado na posição: " + transform.position +
                  " | Valor: " + pointValue + " pts");
    }

    private void ApplyDifficultySettings()
    {
        if (DifficultyManager.Instance == null)
        {
            Debug.Log("[BotTarget] DifficultyManager não encontrado. Usando valor do Inspector (modo teste direto).");
            return;
        }

        var settings = DifficultyManager.Instance.GetCurrentSettings();
        pointValue = settings.botPointValue;

        Debug.Log("[BotTarget] Dificuldade aplicada — Points: " + pointValue);
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

    // -------------------------------------------------------------------------
    // MÉTODOS PÚBLICOS
    // -------------------------------------------------------------------------

    public void SetSpawner(BotTargetSpawner spawnerRef)
    {
        if (spawnerRef == null)
        {
            Debug.LogError("[BotTarget] ERRO: SetSpawner recebeu referência null!");
            return;
        }

        spawner = spawnerRef;
        Debug.Log("[BotTarget] Spawner atribuído: " + spawner.gameObject.name);
    }

    // Chamado pelo Bot quando acerta este alvo — retorna os pontos para o Bot gerenciar
    public int GetHit()
    {
        Debug.Log("[BotTarget] Alvo acertado pelo bot! Valor: " + pointValue + " pontos.");

        if (spawner != null)
            spawner.OnTargetDestroyed();
        else
            Debug.LogWarning("[BotTarget] AVISO: Spawner é null ao tentar notificar!");

        Destroy(gameObject);

        return pointValue;
    }
}