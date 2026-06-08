using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;
using System.Collections;

public class PullLogic : MonoBehaviour
{
    [Header("Feedback Visual da Puxada")]
    // Cores para cada nível de força
    public Color colorWeak = Color.green;
    public Color colorMedium = Color.yellow;
    public Color colorStrong = Color.red;

    // Nome da propriedade de cor no shader (_BaseColor para URP, _Color para Built-in)
    public string colorPropertyName = "_BaseColor";

    [Header("Configurações de Puxada")]
    public Transform nockRestPoint;
    public float maxPullDistance = 0.5f;
    public float maxLaunchForce = 50f;

    [Header("Input")]
    [Tooltip("Ação de gatilho para puxar e disparar (geralmente XRI LeftHand/Activate ou RightHand/Activate)")]
    public UnityEngine.InputSystem.InputActionReference fireAction;

    private Vector3 nockOriginLocalPos;
    private float currentPullAmount = 0f;
    private Transform bowTransform;
    private bool isPulling = false;

    // Renderer da flecha atual — atribuído pelo BowLogic ao encaixar
    private Renderer currentArrowRenderer;

    public float CurrentPullAmount => currentPullAmount;
    public bool IsPulling => isPulling;

    public void Initialize(Transform bow, Vector3 originLocalPos, float force)
    {
        bowTransform = bow;
        nockOriginLocalPos = originLocalPos;
        maxLaunchForce = force;
        Debug.Log($"PullLogic Initialized: Bow={bow.name}, OriginLocal={nockOriginLocalPos}, MaxForce={force}");
    }

    // Chamado pelo BowLogic ao encaixar a flecha — passa o renderer da flecha atual
    public void SetArrowRenderer(Renderer renderer)
    {
        currentArrowRenderer = renderer;

        if (currentArrowRenderer == null)
            Debug.LogWarning("[PullLogic] SetArrowRenderer: renderer recebido é null.");
        else
            Debug.Log("[PullLogic] SetArrowRenderer: renderer atribuído — " + currentArrowRenderer.gameObject.name);

        // Reseta a cor para verde ao encaixar
        ResetArrowColor();
    }

    /// <summary>
    /// Calcula a puxada baseada no gatilho. Retorna true se a flecha deve ser disparada.
    /// </summary>
    public bool ProcessPull(XRGrabInteractable arrowInteractable)
    {
        if (arrowInteractable == null || bowTransform == null) return false;

        IXRInteractor pullingHand = null;
        foreach (var interactor in arrowInteractable.interactorsSelecting)
        {
            if (!(interactor is XRSocketInteractor)) { pullingHand = interactor; break; }
        }
        if (pullingHand == null)
        {
            foreach (var interactor in arrowInteractable.interactorsHovering)
            {
                if (!(interactor is XRSocketInteractor)) { pullingHand = interactor; break; }
            }
        }

        if (pullingHand == null) pullingHand = GetHandInteractor(arrowInteractable);

        if (pullingHand != null)
        {
            bool triggerPressed = IsTriggerPressed(pullingHand);

            if (triggerPressed)
            {
                isPulling = true;

                Vector3 handWorldPos = pullingHand.transform.position;
                Vector3 handLocalPos = bowTransform.InverseTransformPoint(handWorldPos);

                float zDiff = handLocalPos.z - nockOriginLocalPos.z;
                float pullZ = Mathf.Clamp(zDiff, 0, maxPullDistance);
                currentPullAmount = pullZ / maxPullDistance;

                Debug.Log($"[PULL DEBUG] pullZ={pullZ:F4}, currentPull={currentPullAmount:F4}, maxDist={maxPullDistance:F2}");

                // Atualiza a cor da flecha
                UpdateArrowColor();

                nockRestPoint.localPosition = new Vector3(nockOriginLocalPos.x, nockOriginLocalPos.y, nockOriginLocalPos.z + pullZ);

                return false;
            }
            else
            {
                if (isPulling)
                {
                    bool shouldFire = currentPullAmount > 0.1f;
                    isPulling = false;
                    return shouldFire;
                }
            }
        }
        return false;
    }

    public void FireArrow(GameObject arrow, XRGrabInteractable interactable, XRSocketInteractor socket)
    {
        if (arrow == null) return;

        float force = currentPullAmount * maxLaunchForce;
        Vector3 shootDirection = nockRestPoint.forward;

        Debug.Log($"[FIRE] Iniciando disparo: Força={force:F2}, Direção={shootDirection}, Pull={currentPullAmount:F2}");

        // 1. Desativar o socket
        if (socket != null)
        {
            socket.socketActive = false;
            socket.enabled = false;
            StartCoroutine(ReenableSocket(socket));
        }

        // 2. Forçar saída de todos os seletores
        if (interactable != null)
        {
            if (interactable.interactionManager != null)
            {
                var selectors = new List<IXRSelectInteractor>(interactable.interactorsSelecting);
                foreach (var selector in selectors)
                {
                    interactable.interactionManager.SelectExit(selector, interactable);
                }
            }
            interactable.enabled = false;
            StartCoroutine(ReenableInteractable(interactable));
        }

        // 3. Física e Desacoplamento
        arrow.transform.SetParent(null, true);

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            arrow.transform.position += shootDirection * 0.05f;
            rb.AddForce(shootDirection * force, ForceMode.Impulse);
            Debug.Log($"[FIRE] Impulso aplicado. Nova Velocidade: {rb.linearVelocity.magnitude:F2} m/s");
        }

        // 4. Ignorar Colisões Temporariamente
        Collider arrowCol = arrow.GetComponent<Collider>();
        Collider bowCol = bowTransform.GetComponent<Collider>();
        if (arrowCol && bowCol)
        {
            StartCoroutine(IgnoreCollisionTemporarily(arrowCol, bowCol));
        }

        // Limpa o renderer antes de resetar
        currentArrowRenderer = null;

        ResetImmediate();
    }

    private IEnumerator ReenableSocket(XRSocketInteractor socket)
    {
        yield return new WaitForSeconds(0.5f);
        if (socket != null)
        {
            socket.enabled = true;
            socket.socketActive = true;
        }
    }

    private IEnumerator ReenableInteractable(XRGrabInteractable interactable)
    {
        yield return new WaitForSeconds(0.2f);
        if (interactable != null) interactable.enabled = true;
    }

    private IEnumerator IgnoreCollisionTemporarily(Collider a, Collider b)
    {
        Physics.IgnoreCollision(a, b, true);
        yield return new WaitForSeconds(0.5f);
        if (a != null && b != null)
            Physics.IgnoreCollision(a, b, false);
    }

    public void ResetPull()
    {
        if (nockRestPoint != null)
        {
            nockRestPoint.localPosition = Vector3.Lerp(nockRestPoint.localPosition, nockOriginLocalPos, Time.deltaTime * 20f);
        }
        currentPullAmount = 0f;
        isPulling = false;
    }

    public void ResetImmediate()
    {
        if (nockRestPoint != null)
            nockRestPoint.localPosition = nockOriginLocalPos;
        currentPullAmount = 0f;
        isPulling = false;
    }

    private IXRSelectInteractor GetHandInteractor(XRGrabInteractable interactable)
    {
        if (interactable == null) return null;
        foreach (var interactor in interactable.interactorsSelecting)
        {
            if (interactor is XRSocketInteractor) return interactor;
        }
        return null;
    }

    private bool IsTriggerPressed(IXRInteractor interactor)
    {
        if (fireAction != null && fireAction.action != null)
        {
            float val = fireAction.action.ReadValue<float>();
            return val > 0.1f;
        }
        return false;
    }

    public bool IsBeingHeld(XRGrabInteractable interactable)
    {
        if (interactable == null) return false;
        foreach (var interactor in interactable.interactorsSelecting)
        {
            if (interactor is XRBaseInputInteractor) return true;
        }
        return false;
    }

    // Atualiza a cor baseado no currentPullAmount atual
    private void UpdateArrowColor()
    {
        if (currentArrowRenderer == null) return;
        if (currentArrowRenderer.material == null) return;

        if (!currentArrowRenderer.material.HasProperty(colorPropertyName))
        {
            Debug.LogError("[PullLogic] COR: Shader '" + currentArrowRenderer.material.shader.name +
                           "' não tem a propriedade '" + colorPropertyName + "'. " +
                           "Troca para '_Color' (Built-in) ou '_BaseColor' (URP).");
            return;
        }

        Color targetColor;

        if (currentPullAmount < 0.4f)
        {
            float t = currentPullAmount / 0.4f;
            targetColor = Color.Lerp(colorWeak, colorMedium, t);
        }
        else if (currentPullAmount < 0.8f)
        {
            float t = (currentPullAmount - 0.4f) / 0.4f;
            targetColor = Color.Lerp(colorMedium, colorStrong, t);
        }
        else
        {
            targetColor = colorStrong;
        }

        currentArrowRenderer.material.SetColor(colorPropertyName, targetColor);

        Debug.Log("[PullLogic] COR: Pull=" + currentPullAmount.ToString("F2") +
                  " | Cor=" + targetColor);
    }

    // Reseta a cor para verde
    private void ResetArrowColor()
    {
        if (currentArrowRenderer == null) return;
        if (currentArrowRenderer.material == null) return;

        currentArrowRenderer.material.SetColor(colorPropertyName, colorWeak);
        Debug.Log("[PullLogic] COR RESET: cor resetada para verde.");
    }
}