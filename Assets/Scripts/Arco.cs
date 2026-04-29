using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Bow : MonoBehaviour
{
    [Header("Referências")]
    public Transform nockRestPoint;
    public Transform pullPoint;
    public XRGrabInteractable pullPointInteractable;

    private GameObject nockedArrow;
    private Rigidbody arrowRigidbody;

    void Start()
    {
        // Começa com pullPoint desabilitado
        if (pullPointInteractable != null)
            pullPointInteractable.enabled = false;
    }

    public void NockArrow(GameObject arrow)
    {
        // Se já há flecha, força liberação (pode ser que a anterior tenha sido removida manualmente)
        if (nockedArrow != null)
        {
            Debug.Log("Substituindo flecha nockada.");
            ForceReleaseArrow();
        }

        nockedArrow = arrow;
        arrowRigidbody = arrow.GetComponent<Rigidbody>();

        if (arrowRigidbody != null)
        {
            arrowRigidbody.isKinematic = true;
            arrowRigidbody.useGravity = false;
        }

        // Flecha vira filha do pullPoint (opção mais fácil)
        arrow.transform.SetParent(pullPoint);
        arrow.transform.localPosition = Vector3.zero;
        arrow.transform.localRotation = Quaternion.identity;

        // Guarda referência do arco na flecha, se tiver script Arrow
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
            arrowScript.currentBow = this;

        // Habilita interação com pullPoint
        if (pullPointInteractable != null)
        {
            pullPointInteractable.enabled = true;
            Debug.Log("PullPoint habilitado para agarrar.");
        }
    }

    public void ReleaseArrow()
    {
        if (nockedArrow == null) return;

        // Calcula força de disparo baseada na distância entre pullPoint e nockRestPoint
        float pullDistance = Vector3.Distance(pullPoint.position, nockRestPoint.position);
        Vector3 shootDirection = (nockRestPoint.position - transform.position).normalized;
        float force = pullDistance * 500f; // ajuste conforme necessidade

        // Desprende a flecha
        nockedArrow.transform.SetParent(null);
        if (arrowRigidbody != null)
        {
            arrowRigidbody.isKinematic = false;
            arrowRigidbody.useGravity = true;
            arrowRigidbody.AddForce(shootDirection * force, ForceMode.Impulse);
        }

        // Limpa referência da flecha
        Arrow arrowScript = nockedArrow.GetComponent<Arrow>();
        if (arrowScript != null)
            arrowScript.currentBow = null;

        nockedArrow = null;
        arrowRigidbody = null;

        // Desabilita pullPoint e reseta posição
        if (pullPointInteractable != null)
        {
            pullPointInteractable.enabled = false;
        }
        pullPoint.localPosition = Vector3.zero; // volta ao centro

        Debug.Log("Flecha disparada.");
    }

    public void ForceReleaseArrow()
    {
        if (nockedArrow == null) return;

        nockedArrow.transform.SetParent(null);
        if (arrowRigidbody != null)
        {
            arrowRigidbody.isKinematic = false;
            arrowRigidbody.useGravity = true;
        }

        Arrow arrowScript = nockedArrow.GetComponent<Arrow>();
        if (arrowScript != null)
            arrowScript.currentBow = null;

        nockedArrow = null;
        arrowRigidbody = null;

        if (pullPointInteractable != null)
            pullPointInteractable.enabled = false;
        pullPoint.localPosition = Vector3.zero;

        Debug.Log("Flecha removida manualmente do arco.");
    }

    // Chamado pelo evento do XR Grab Interactable do pullPoint
    public void OnPullPointReleased()
    {
        ReleaseArrow();
    }
}