using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BowLogic : MonoBehaviour
{
    [Header("Referências")]
    public LineRenderer lineRenderer;
    public Transform pullPoint;
    public Transform stringStartPoint;
    public Transform nockRestPoint;
    public Transform topPoint;
    public Transform bottomPoint;

    [Header("Configurações")]
    public float maxPullDistance = 0.6f;
    public float maxLaunchForce = 40f;

    // Ajuste isso no Inspector se a flecha ficar "torta" ao encaixar
    public Vector3 arrowRotationOffset = new Vector3(0, 0, 0);

    private GameObject currentArrow;
    private XRGrabInteractable arrowInteractable;
    private float currentPullAmount;

    void Update()
    {
        if (currentArrow != null && arrowInteractable != null && arrowInteractable.isSelected)
        {
            // 1. Pegamos a posição da mão que segura a flecha
            Vector3 handPos = arrowInteractable.interactorsSelecting[0].transform.position;

            // 2. Movemos a corda (pullPoint)
            UpdateStringPosition(handPos);
        }
        else if (currentArrow != null)
        {
            // Se soltou a flecha enquanto ela estava no arco: ATIRA
            ReleaseArrow();
        }
        else
        {
            // Se não tem flecha, a corda volta ao normal
            ResetString();
        }

        UpdateStringVisuals();
    }

    public void UpdateStringPosition(Vector3 handWorldPos)
    {
        // IMPORTANTE: Usamos o transform do ARCO como referência
        Vector3 localHandPos = transform.InverseTransformPoint(handWorldPos);

        // Clamp para a corda só ir para trás no eixo Z
        float zPull = Mathf.Clamp(localHandPos.z, -maxPullDistance, 0);

        pullPoint.localPosition = new Vector3(0, 0, zPull);
        currentPullAmount = Mathf.Abs(zPull) / maxPullDistance;
    }

    // Chame esta função no evento "Select Entered" do seu XRSocketInteractor (ou via Trigger)
    public void OnArrowNocked(SelectEnterEventArgs args)
    {
        currentArrow = args.interactableObject.transform.gameObject;
        arrowInteractable = currentArrow.GetComponent<XRGrabInteractable>();

        // CONFIGURAÇÃO MÁGICA:
        // Torna a flecha filha do pullPoint para que ela siga a corda perfeitamente
        currentArrow.transform.SetParent(pullPoint);

        // Zera a posição e aplica a rotação do nockPoint
        currentArrow.transform.localPosition = Vector3.zero;
        currentArrow.transform.localRotation = Quaternion.Euler(arrowRotationOffset);

        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    public void ReleaseArrow()
    {
        if (currentArrow != null)
        {
            // Tira a flecha de "filha" do arco antes de aplicar força
            currentArrow.transform.SetParent(null);

            Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            if (currentPullAmount > 0.1f)
            {
                rb.AddForce(nockRestPoint.forward * (currentPullAmount * maxLaunchForce), ForceMode.Impulse);
            }

            currentArrow = null;
            arrowInteractable = null;
        }
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