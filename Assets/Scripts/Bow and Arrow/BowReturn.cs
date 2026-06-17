using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class BowReturn : MonoBehaviour
{
    [Header("Configurações")]
    // Tempo em segundos até o arco voltar após ser solto
    public float returnDelay = 4f;

    // Velocidade com que o arco volta para o spawn
    public float returnSpeed = 5f;

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS
    // -------------------------------------------------------------------------

    // Posição e rotação originais do arco (definidas no Start)
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    // Referência ao XRGrabInteractable do arco
    private XRGrabInteractable grabInteractable;

    // Referência ao Rigidbody do arco
    private Rigidbody rb;

    // Controla se o arco está retornando
    private bool isReturning = false;

    // Coroutine atual de retorno (para poder cancelar se o player pegar de volta)
    private Coroutine returnCoroutine;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        // Salva a posição e rotação iniciais como ponto de spawn
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        Debug.Log("[BowReturn] Spawn salvo em: " + spawnPosition);

        // Pega os componentes necessários
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (grabInteractable == null)
        {
            Debug.LogError("[BowReturn] ERRO: XRGrabInteractable não encontrado no arco! " +
                           "O script BowReturn deve estar no mesmo GameObject que o XRGrabInteractable.");
            return;
        }

        if (rb == null)
            Debug.LogWarning("[BowReturn] AVISO: Rigidbody não encontrado. " +
                             "O arco pode não se mover corretamente ao retornar.");

        // Escuta os eventos de pegar e soltar
        grabInteractable.selectEntered.AddListener(OnBowGrabbed);
        grabInteractable.selectExited.AddListener(OnBowReleased);

        Debug.Log("[BowReturn] Iniciado. O arco voltará ao spawn " + returnDelay + "s após ser solto.");
    }

    void OnDestroy()
    {
        // Remove os listeners ao destruir para evitar memory leaks
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnBowGrabbed);
            grabInteractable.selectExited.RemoveListener(OnBowReleased);
        }
    }

    // -------------------------------------------------------------------------
    // EVENTOS DE GRAB
    // -------------------------------------------------------------------------

    // Chamado quando o player pega o arco
    private void OnBowGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("[BowReturn] Arco pegado pelo player. Cancelando retorno se estava em curso.");

        isReturning = false;

        // Cancela a coroutine de retorno se estava rodando
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }

    // Chamado quando o player solta o arco
    private void OnBowReleased(SelectExitEventArgs args)
    {
        Debug.Log("[BowReturn] Arco solto. Retornando ao spawn em " + returnDelay + "s...");

        // Inicia a contagem para retornar
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);

        returnCoroutine = StartCoroutine(ReturnRoutine());
    }

    // -------------------------------------------------------------------------
    // COROUTINE DE RETORNO
    // -------------------------------------------------------------------------

    private IEnumerator ReturnRoutine()
    {
        // Aguarda o delay antes de começar a mover
        float elapsed = 0f;
        while (elapsed < returnDelay)
        {
            // Se o player pegou o arco durante a espera, para tudo
            if (grabInteractable.isSelected)
            {
                Debug.Log("[BowReturn] Arco foi pego durante o delay. Retorno cancelado.");
                yield break;
            }

            elapsed += Time.deltaTime;
            Debug.Log("[BowReturn] Retornando em: " + (returnDelay - elapsed).ToString("F1") + "s");
            yield return null;
        }

        Debug.Log("[BowReturn] Iniciando movimento de retorno ao spawn...");

        isReturning = true;

        // Desativa física para mover suavemente
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Move suavemente até o spawn
        while (isReturning)
        {
            // Se o player pegou durante o retorno, para
            if (grabInteractable.isSelected)
            {
                Debug.Log("[BowReturn] Arco foi pego durante o retorno.");
                isReturning = false;

                // Reativa física
                if (rb != null)
                    rb.isKinematic = false;

                yield break;
            }

            // Interpola posição e rotação suavemente
            transform.position = Vector3.MoveTowards(
                transform.position,
                spawnPosition,
                returnSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                spawnRotation,
                returnSpeed * 50f * Time.deltaTime
            );

            // Verifica se chegou ao spawn
            float distanceToSpawn = Vector3.Distance(transform.position, spawnPosition);
            if (distanceToSpawn < 0.01f)
            {
                // Posiciona exatamente no spawn
                transform.position = spawnPosition;
                transform.rotation = spawnRotation;
                isReturning = false;

                // Reativa física
                if (rb != null)
                    rb.isKinematic = false;

                Debug.Log("[BowReturn] Arco retornou ao spawn com sucesso!");
            }

            yield return null;
        }
    }
}
