using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BowLogic : MonoBehaviour
{
    [Header("Referências")]
    public LineRenderer lineRenderer;
    public Transform pullPoint;
    public Transform stringStartPoint;
    public Transform nockRestPoint; // A seta azul (Z) deve apontar para frente do arco
    public Transform topPoint;
    public Transform bottomPoint;

    [Header("Configurações")]
    public float maxPullDistance = 0.6f;
    public float maxLaunchForce = 40f;

    [Tooltip("Ajuste isso para a flecha não ficar paralela (ex: 90 no X ou Y)")]
    public Vector3 arrowRotationOffset;

    private GameObject currentArrow;
    private XRGrabInteractable arrowInteractable;
    private float currentPullAmount;
    private Collider bowCollider;

    void Start()
    {
        // Pega o colisor do arco para ignorar a flecha depois
        bowCollider = GetComponentInChildren<Collider>();
    }

    void Update()
    {
        // Se houver uma flecha encaixada
        if (currentArrow != null && arrowInteractable != null)
        {
            // Verificamos se ALGUMA mão ainda está segurando a flecha
            // Usamos interactorsSelecting.Count > 0 porque o Socket também conta como um interactor
            if (arrowInteractable.interactorsSelecting.Count > 0)
            {
                // Pegamos a posição do primeiro interactor (que deve ser sua mão)
                // Se a flecha estiver no socket, o socket é o interactor 0, a mão é o 1
                var interactor = arrowInteractable.interactorsSelecting[arrowInteractable.interactorsSelecting.Count - 1];

                Vector3 handPos = interactor.transform.position;
                UpdateStringPosition(handPos);

                // Mantém a flecha grudada na corda puxada
                currentArrow.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            ResetString();
        }

        UpdateStringVisuals();
    }

    public void OnArrowNocked(SelectEnterEventArgs args)
    {
        currentArrow = args.interactableObject.transform.gameObject;
        arrowInteractable = currentArrow.GetComponent<XRGrabInteractable>();

        // --- SOLUÇÃO PARA O ARCO NÃO VOAR ---
        // Pega todos os colisores do ARCO e da FLECHA
        Collider[] bowColliders = GetComponentsInChildren<Collider>();
        Collider[] arrowColliders = currentArrow.GetComponentsInChildren<Collider>();

        // Diz para a Unity ignorar a colisão entre CADA parte do arco e CADA parte da flecha
        foreach (var b in bowColliders)
        {
            foreach (var a in arrowColliders)
            {
                Physics.IgnoreCollision(a, b, true);
            }
        }
        // ------------------------------------

        currentArrow.transform.SetParent(pullPoint);
        currentArrow.transform.localPosition = Vector3.zero;
        currentArrow.transform.rotation = nockRestPoint.rotation * Quaternion.Euler(arrowRotationOffset);

        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false; // Garante que a gravidade não puxe a flecha pra baixo
        }
    }

    public void ReleaseArrow()
    {
        if (currentArrow != null)
        {
            Debug.Log("Atirando flecha com força: " + (currentPullAmount * maxLaunchForce));

            // 1. Desvincula a flecha do pullPoint (corda)
            currentArrow.transform.SetParent(null);

            // 2. Reativa a física
            Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

                // 3. Aplica a força na direção frontal do nockRestPoint
                // Se currentPullAmount for quase 0, não atira (evita tiros por erro)
                if (currentPullAmount > 0.05f)
                {
                    Vector3 forcaDisparo = nockRestPoint.forward * (currentPullAmount * maxLaunchForce);
                    rb.AddForce(forcaDisparo, ForceMode.Impulse);

                    // Opcional: Adiciona um torque para a flecha girar levemente (estabiliza)
                    rb.AddRelativeTorque(Vector3.forward * 10, ForceMode.Impulse);
                }
            }

            // 4. Limpa as referências para poder pegar a próxima flecha
            currentArrow = null;
            arrowInteractable = null;

            // 5. Faz a corda voltar ao centro
            ResetString();
        }
    }

    // ... (Mantenha suas funções UpdateStringPosition e UpdateStringVisuals iguais)
    public void UpdateStringPosition(Vector3 handWorldPos)
    {
        Vector3 localHandPos = transform.InverseTransformPoint(handWorldPos);
        float zPull = Mathf.Clamp(localHandPos.z, -maxPullDistance, 0);
        pullPoint.localPosition = new Vector3(0, 0, zPull);
        currentPullAmount = Mathf.Abs(zPull) / maxPullDistance;
    }

    void UpdateStringVisuals()
    {
        lineRenderer.SetPosition(0, topPoint.position);
        lineRenderer.SetPosition(1, pullPoint.position);
        lineRenderer.SetPosition(2, bottomPoint.position);
    }

    private void ResetString()
    {
        pullPoint.localPosition = Vector3.Lerp(pullPoint.localPosition, stringStartPoint.localPosition, Time.deltaTime * 20f);
    }
}