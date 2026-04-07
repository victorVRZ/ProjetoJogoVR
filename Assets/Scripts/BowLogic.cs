using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BowLogic : MonoBehaviour
{
    public XRGrabInteractable interactable;

    [Header("Referencias da Corda")]
    public LineRenderer lineRenderer;
    public Transform topPoint;
    public Transform bottomPoint;
    public Transform pullPoint;

    [Header("Configurações de Puxada da Corda")]
    public Transform nockRestPoint;
    public float maxPullDistance = 0.6f;
    public float maxLaunchForce = 40f;

    private float currentPullAmount;
    void Start()
    {
        
    }

    void Update()
    {
        // Se o pullPoint estiver sendo "segurado" pelo sistema de VR
        if (interactable.isSelected)
        {
            // Pega a posição da mão que está segurando
            Vector3 handPos = interactable.interactorsSelecting[0].transform.position;
            UpdateStringPosition(handPos);
        }
        else
        {
            // Se soltou, a corda volta pro zero (visual)
            pullPoint.localPosition = Vector3.Lerp(pullPoint.localPosition, Vector3.zero, Time.deltaTime * 20);
        }

        UpdateStringVisuals();
    }

    public void UpdateStringPosition(Vector3 handWorldPos) 
    {
        // converte a posicao da mao de world para local
        Vector3 localHandPos = nockRestPoint.InverseTransformPoint(handWorldPos);

        //capa o movimento da mao impedindo que ultrapasse o ponto de repouso e o ponto maximo e impede de ir para os lados
        float stringLimit = Mathf.Clamp(localHandPos.z, -maxPullDistance, 0);

        //atualiza o pullpoint da corda (apenas visual)
        pullPoint.localPosition = new Vector3(0, 0, stringLimit);

        //calcula força
        currentPullAmount = Mathf.Abs(stringLimit) / maxPullDistance;
        
    }
    void UpdateStringVisuals()
    {
        // Desenha a linha entre as pontas e o ponto de puxada
        lineRenderer.SetPosition(0, topPoint.position);
        lineRenderer.SetPosition(1, pullPoint.position);
        lineRenderer.SetPosition(2, bottomPoint.position);
    }

    public void ReleaseArrow(Rigidbody arrowRb)
    {
        if (currentPullAmount > 0.1f)
        {
            float force = currentPullAmount * maxLaunchForce;
            // Dispara na direção 'forward' do arco
            arrowRb.AddForce(nockRestPoint.forward * force, ForceMode.Impulse);
        }

        // Reseta a corda para o centro
        currentPullAmount = 0;
        pullPoint.localPosition = Vector3.zero;
    }
}

