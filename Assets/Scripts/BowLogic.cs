using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections;
using System.Collections.Generic;

public class BowLogic : MonoBehaviour
{
    [Header("Pontos do Arco")]
    public Transform nockRestPoint;
    public Transform topPoint;
    public Transform bottomPoint;

    [Header("Visual")]
    public LineRenderer lineRenderer;

    [Header("Configurações")]
    public float maxLaunchForce = 50f;
    public Vector3 arrowRotationOffset;
    public string arrowTag = "Arrow";

    [Header("Lógica de Puxada")]
    public PullLogic pullLogic;

    private Vector3 nockOriginLocalPos;
    private GameObject currentArrow;
    private XRGrabInteractable arrowInteractable;
    private Transform arrowAttachTransform;
    private bool isNocked = false;

    void Start()
    {
        if (nockRestPoint != null)
        {
            nockOriginLocalPos = nockRestPoint.localPosition;
        }

        // Configuração obrigatória do LineRenderer
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 3;
            lineRenderer.useWorldSpace = true; // Fundamental para SetPosition(worldPos) funcionar
            Debug.Log("BowLogic: LineRenderer configurado com 3 pontos e World Space.");
        }

        // Se o PullLogic não estiver atribuído, tenta achar no mesmo objeto
        if (pullLogic == null) pullLogic = GetComponent<PullLogic>();
        
        if (pullLogic != null)
        {
            pullLogic.Initialize(transform, nockOriginLocalPos, maxLaunchForce);
        }
        else
        {
            Debug.LogError("BowLogic: PullLogic não encontrado!");
        }

        var socket = GetComponentInChildren<XRSocketInteractor>();
        if (socket != null)
        {
            socket.selectEntered.AddListener(OnArrowNocked);
            socket.selectExited.AddListener(OnArrowUnnocked);
        }
    }

    void Update()
    {
        
        if (isNocked && currentArrow != null && pullLogic != null)
            // ... resto do código
            if (isNocked && currentArrow != null && pullLogic != null)
        {
            if (pullLogic.ProcessPull(arrowInteractable))
            {
                Fire();
            }
            else
            {
                if (!pullLogic.IsPulling)
                {
                    pullLogic.ResetPull();
                }
                UpdateArrowTransform();
            }
        }
        else
        {
            if (pullLogic != null) pullLogic.ResetPull();
        }

        UpdateStringVisuals();
    }

    private void Fire()
    {
        if (currentArrow == null || pullLogic == null) return;

        GameObject arrowToFire = currentArrow;
        XRGrabInteractable interactableToFire = arrowInteractable;
        var socket = GetComponentInChildren<XRSocketInteractor>();

        // 1. Limpamos o estado do BowLogic PRIMEIRO para parar o UpdateArrowTransform
        isNocked = false;
        currentArrow = null;
        arrowInteractable = null;
        arrowAttachTransform = null;

        // 2. Delegamos o disparo físico e a limpeza de XR ao PullLogic
        pullLogic.FireArrow(arrowToFire, interactableToFire, socket);
    }

    private void UpdateArrowTransform()
    {
        if (currentArrow == null || nockRestPoint == null) return;

        // A flecha deve seguir a posição e a rotação do ponto de encaixe (nockRestPoint)
        currentArrow.transform.position = nockRestPoint.position;
        
        // Alinhamento visual: A flecha deve apontar para onde o arco aponta (Z+)
        // Usamos o forward do arco para garantir que a flecha não "balance" com a mão
        Vector3 bowForward = transform.forward;
        currentArrow.transform.rotation = Quaternion.LookRotation(bowForward) * Quaternion.Euler(arrowRotationOffset);

        if (arrowAttachTransform != null)
        {
            // Se a flecha tiver um ponto de encaixe customizado, compensamos a rotação local dele
            currentArrow.transform.rotation *= Quaternion.Inverse(arrowAttachTransform.localRotation);
        }
    }

    // Método Fire removido pois a lógica agora está no PullLogic

    public void OnArrowNocked(SelectEnterEventArgs args)
    {
        if (args.interactableObject == null) return;
        SetupNockedArrow(args.interactableObject.transform.gameObject);
    }

    public void NockManual(GameObject arrow)
    {
        if (isNocked || arrow == null) return;
        SetupNockedArrow(arrow);
    }

    private void SetupNockedArrow(GameObject arrow)
    {
        currentArrow = arrow;
        arrowInteractable = arrow.GetComponent<XRGrabInteractable>();

        if (arrowInteractable != null)
            arrowAttachTransform = arrowInteractable.attachTransform;

        isNocked = true;

        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // LINHA NOVA — passa o renderer da flecha para o PullLogic
        if (pullLogic != null)
        {
            Renderer arrowRend = arrow.GetComponentInChildren<Renderer>();
            pullLogic.SetArrowRenderer(arrowRend);
        }

        UpdateArrowTransform();
    }

    public void OnArrowUnnocked(SelectExitEventArgs args)
    {
        if (currentArrow != null && args.interactableObject.transform.gameObject == currentArrow)
        {
            if (pullLogic != null && !pullLogic.IsBeingHeld(arrowInteractable))
            {
                isNocked = false;
                currentArrow = null;
                arrowInteractable = null;
                arrowAttachTransform = null;
            }
        }
    }

    private void UpdateStringVisuals()
    {
        if (lineRenderer != null && topPoint != null && nockRestPoint != null && bottomPoint != null)
        {
            lineRenderer.SetPosition(0, topPoint.position);
            lineRenderer.SetPosition(1, nockRestPoint.position);
            lineRenderer.SetPosition(2, bottomPoint.position);
        }
    }
}