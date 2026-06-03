using UnityEngine;

/// <summary>
/// Script central de controle do sistema de arco e flecha.
/// Não substitui nenhum script existente — apenas expõe e sincroniza
/// as propriedades principais de BowLogic, PullLogic e ArrowNockDetector.
/// </summary>
public class BowArrowController : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // REFERÊNCIAS AOS SCRIPTS EXISTENTES
    // -------------------------------------------------------------------------

    [Header("Referências (auto-detectadas se vazias)")]
    // Referência ao BowLogic — controla pontos do arco e disparo
    public BowLogic bowLogic;

    // Referência ao PullLogic — controla mecânica de puxada
    public PullLogic pullLogic;

    // Referência ao ArrowNockDetector — controla encaixe da flecha
    public ArrowNockDetector arrowNockDetector;

    // -------------------------------------------------------------------------
    // CONFIGURAÇÕES DO BOWLOGIC
    // -------------------------------------------------------------------------

    [Header("BowLogic — Configurações do Arco")]

    // Força máxima de lançamento da flecha (m/s com ForceMode.Impulse)
    [Range(10f, 150f)]
    public float maxLaunchForce = 50f;

    // Tag usada para identificar flechas na cena
    public string arrowTag = "Arrow";

    // Offset de rotação visual da flecha quando encaixada no arco
    public Vector3 arrowRotationOffset = Vector3.zero;

    // -------------------------------------------------------------------------
    // CONFIGURAÇÕES DO PULLLOGIC
    // -------------------------------------------------------------------------

    [Header("PullLogic — Configurações de Puxada")]

    // Distância máxima que a corda pode ser puxada (em metros)
    [Range(0.1f, 1f)]
    public float maxPullDistance = 0.5f;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Awake()
    {
        Debug.Log("[BowArrowController] Iniciando auto-detecção de scripts...");
        AutoDetectScripts();
    }

    void Start()
    {
        Debug.Log("[BowArrowController] Aplicando configurações iniciais...");
        ApplyAll();
    }

    // -------------------------------------------------------------------------
    // MÉTODOS PRIVADOS
    // -------------------------------------------------------------------------

    // Tenta encontrar os scripts automaticamente se não foram atribuídos no Inspector
    void AutoDetectScripts()
    {
        if (bowLogic == null)
        {
            bowLogic = GetComponentInChildren<BowLogic>();
            if (bowLogic != null)
                Debug.Log("[BowArrowController] BowLogic encontrado automaticamente: " + bowLogic.gameObject.name);
            else
                Debug.LogError("[BowArrowController] ERRO: BowLogic não encontrado! " +
                               "Arraste o arco no campo 'Bow Logic' no Inspector.");
        }

        if (pullLogic == null)
        {
            pullLogic = GetComponentInChildren<PullLogic>();
            if (pullLogic != null)
                Debug.Log("[BowArrowController] PullLogic encontrado automaticamente: " + pullLogic.gameObject.name);
            else
                Debug.LogError("[BowArrowController] ERRO: PullLogic não encontrado! " +
                               "Arraste o script PullLogic no campo 'Pull Logic' no Inspector.");
        }

        if (arrowNockDetector == null)
        {
            arrowNockDetector = GetComponentInChildren<ArrowNockDetector>();
            if (arrowNockDetector != null)
                Debug.Log("[BowArrowController] ArrowNockDetector encontrado automaticamente: " + arrowNockDetector.gameObject.name);
            else
                Debug.LogWarning("[BowArrowController] AVISO: ArrowNockDetector não encontrado. " +
                                 "Se usar encaixe manual, arraste-o no Inspector.");
        }
    }

    // -------------------------------------------------------------------------
    // MÉTODOS PÚBLICOS — aplica configurações individualmente ou todas de uma vez
    // -------------------------------------------------------------------------

    // Aplica TODAS as configurações de uma vez
    public void ApplyAll()
    {
        ApplyBowLogicSettings();
        ApplyPullLogicSettings();
        Debug.Log("[BowArrowController] Todas as configurações aplicadas.");
    }

    // Aplica apenas as configurações do BowLogic
    public void ApplyBowLogicSettings()
    {
        if (bowLogic == null)
        {
            Debug.LogError("[BowArrowController] ERRO: BowLogic é null. Não foi possível aplicar configurações.");
            return;
        }

        bowLogic.maxLaunchForce = maxLaunchForce;
        bowLogic.arrowTag = arrowTag;
        bowLogic.arrowRotationOffset = arrowRotationOffset;

        // Sincroniza a força com o PullLogic via BowLogic
        if (bowLogic.pullLogic != null)
            bowLogic.pullLogic.maxLaunchForce = maxLaunchForce;

        Debug.Log("[BowArrowController] BowLogic atualizado — Força: " + maxLaunchForce +
                  " | Tag: " + arrowTag);
    }

    // Aplica apenas as configurações do PullLogic
    public void ApplyPullLogicSettings()
    {
        if (pullLogic == null)
        {
            Debug.LogError("[BowArrowController] ERRO: PullLogic é null. Não foi possível aplicar configurações.");
            return;
        }

        pullLogic.maxPullDistance = maxPullDistance;
        pullLogic.maxLaunchForce = maxLaunchForce;

        Debug.Log("[BowArrowController] PullLogic atualizado — Distância máxima: " + maxPullDistance +
                  " | Força: " + maxLaunchForce);
    }
}