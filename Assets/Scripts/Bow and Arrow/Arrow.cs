using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Arrow : MonoBehaviour
{

    [Header("Auto Destruição")]
    // Tempo em segundos até a flecha se destruir após ser disparada (0 = nunca)
    public float destroyDelay = 5f;

    // Referência ao spawner que criou esta flecha
    private ArrowSpawner spawner;

    // Controla se já notificou o spawner (evita notificar duas vezes)
    private bool hasNotifiedSpawner = false;

    void Start()
    {
        Debug.Log("[Arrow] Flecha criada: " + gameObject.name + " na posição: " + transform.position);
    }

    // Chamado pelo ArrowSpawner após instanciar
    public void SetSpawner(ArrowSpawner spawnerRef)
    {
        if (spawnerRef == null)
        {
            Debug.LogError("[Arrow] ERRO: SetSpawner recebeu referência null!");
            return;
        }

        spawner = spawnerRef;
        Debug.Log("[Arrow] Spawner atribuído: " + spawner.gameObject.name);
    }

    // Chamado pelo BowLogic ao disparar a flecha (adiciona no FireArrow)
    public void OnFired()
    {
        NotifySpawner();

        // Destrói a flecha após o tempo configurado
        if (destroyDelay > 0f)
        {
            Debug.Log("[Arrow] Flecha será destruída em " + destroyDelay + "s.");
            Destroy(gameObject, destroyDelay);
        }
    }

    // Chamado quando o player pega a flecha sem encaixar no arco
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[Arrow] Flecha pega pelo player.");
            NotifySpawner();
        }
    }

    private void NotifySpawner()
    {
        // Garante que só notifica uma vez
        if (hasNotifiedSpawner) return;
        hasNotifiedSpawner = true;

        if (spawner != null)
            spawner.OnArrowRemoved();
        else
            Debug.LogWarning("[Arrow] AVISO: Spawner é null ao notificar.");
    }
}