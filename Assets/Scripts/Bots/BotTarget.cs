using UnityEngine;

public class BotTarget : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // VARIÁVEIS PÚBLICAS
    // -------------------------------------------------------------------------

    [Header("Movimento")]
    // Velocidade de movimento lateral do alvo
    public float moveSpeed = 2f;

    // Distância máxima que o alvo se afasta da posição inicial
    public float moveRange = 3f;

    [Header("Score")]
    // Pontos que o bot ganha ao acertar este alvo
    public int pointValue = 100;

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS
    // -------------------------------------------------------------------------

    // Posição inicial do alvo para calcular o range
    private Vector3 startPosition;

    // Direção atual do movimento (-1 esquerda, 1 direita)
    private float direction = 1f;

    // Referência ao spawner que criou este alvo
    private BotTargetSpawner spawner;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        startPosition = transform.position;
        Debug.Log("[BotTarget] Alvo criado na posição: " + transform.position);
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

        // Avisa o spawner para recriar o alvo
        if (spawner != null)
            spawner.OnTargetDestroyed();
        else
            Debug.LogWarning("[BotTarget] AVISO: Spawner é null ao tentar notificar!");

        Destroy(gameObject);

        // Retorna o valor dos pontos para o Bot somar no seu próprio score
        return pointValue;
    }
}