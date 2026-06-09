using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    [Header("Configurações do Spawn")]
    // Prefab da flecha (arraste o prefab da flecha aqui)
    public GameObject arrowPrefab;

    // Tempo de espera antes de spawnar nova flecha após a atual ser pega ou disparada
    public float respawnDelay = 1f;

    // Referência à flecha atualmente ativa na cena
    private GameObject currentArrow;

    void Start()
    {
        Debug.Log("[ArrowSpawner] Iniciado em: " + gameObject.name);

        if (arrowPrefab == null)
        {
            Debug.LogError("[ArrowSpawner] ERRO: arrowPrefab não atribuído no Inspector!");
            return;
        }

        SpawnArrow();
    }

    // Chamado pelo Arrow quando é pega ou disparada
    public void OnArrowRemoved()
    {
        currentArrow = null;
        Debug.Log("[ArrowSpawner] Flecha removida. Respawnando em " + respawnDelay + "s...");
        Invoke(nameof(SpawnArrow), respawnDelay);
    }

    private void SpawnArrow()
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("[ArrowSpawner] ERRO: arrowPrefab é null no momento do spawn!");
            return;
        }

        if (currentArrow != null)
        {
            Debug.LogWarning("[ArrowSpawner] Já existe uma flecha ativa. Spawn cancelado.");
            return;
        }

        // Spawna na posição e rotação exata deste GameObject
        currentArrow = Instantiate(arrowPrefab, transform.position, transform.rotation);

        Debug.Log("[ArrowSpawner] Flecha spawnada em: " + transform.position);

        // Passa referência do spawner para a flecha
        Arrow arrowScript = currentArrow.GetComponent<Arrow>();
        if (arrowScript == null)
        {
            Debug.LogError("[ArrowSpawner] ERRO: Prefab não possui o script 'Arrow'!");
            return;
        }

        arrowScript.SetSpawner(this);
        Debug.Log("[ArrowSpawner] Spawn concluído!");
    }
}