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
    public Vector3 arrowRotationOffset = new Vector3(0, 0, 0);

    private GameObject currentArrow;
    private XRGrabInteractable arrowInteractable;
    private float currentPullAmount;

    void Update()
    {
        if (currentArrow != null && arrowInteractable != null && arrowInteractable.isSelected)
        {
            Vector3 handPos = arrowInteractable.interactorsSelecting[0].transform.position;
            UpdateStringPosition(handPos);

            // Força a flecha a ficar no pullPoint
            currentArrow.transform.localPosition = Vector3.zero;
        }
        else if (currentArrow != null)
        {
            ReleaseArrow();
        }
        else
        {
            ResetString();
        }

        UpdateStringVisuals();
    }

    public void UpdateStringPosition(Vector3 handWorldPos)
    {
        Vector3 localHandPos = transform.InverseTransformPoint(handWorldPos);
        float zPull = Mathf.Clamp(localHandPos.z, -maxPullDistance, 0);
        pullPoint.localPosition = new Vector3(0, 0, zPull);
        currentPullAmount = Mathf.Abs(zPull) / maxPullDistance;
    }

    // ESSA FUNÇÃO PRECISA APARECER NO CONSOLE
    public void OnArrowNocked(SelectEnterEventArgs args)
    {
        Debug.Log("Tentando encaixar flecha: " + args.interactableObject.transform.name);

        currentArrow = args.interactableObject.transform.gameObject;
        arrowInteractable = currentArrow.GetComponent<XRGrabInteractable>();

        if (arrowInteractable == null)
        {
            Debug.LogError("O objeto encaixado não tem um XRGrabInteractable!");
            return;
        }

        // Parentesco
        currentArrow.transform.SetParent(pullPoint);
        currentArrow.transform.localPosition = Vector3.zero;
        currentArrow.transform.localRotation = Quaternion.Euler(arrowRotationOffset);

        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Debug.Log("Flecha encaixada com sucesso!");
    }

    public void ReleaseArrow()
    {
        if (currentArrow != null)
        {
            Debug.Log("Disparando flecha!");
            currentArrow.transform.SetParent(null);

            Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
                rb.AddForce(nockRestPoint.forward * (currentPullAmount * maxLaunchForce), ForceMode.Impulse);
            }

            currentArrow = null;
            arrowInteractable = null;
        }
    }

    void UpdateStringVisuals()
    {
        if (lineRenderer == null) return;
        lineRenderer.SetPosition(0, topPoint.position);
        lineRenderer.SetPosition(1, pullPoint.position);
        lineRenderer.SetPosition(2, bottomPoint.position);
    }

    private void ResetString()
    {
        pullPoint.localPosition = Vector3.Lerp(pullPoint.localPosition, stringStartPoint.localPosition, Time.deltaTime * 20f);
    }
}