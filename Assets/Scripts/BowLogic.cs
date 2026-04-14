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
    public Transform StringStartPoint;

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
        // Verifica se o jogador está segurando a corda/flecha
        if (interactable != null && interactable.isSelected)
        {
            // Pega a posição da mão (interactor) que está selecionando o objeto
            Vector3 handPos = interactable.interactorsSelecting[0].transform.position;
            UpdateStringPosition(handPos);
        }
        else
        {
            // Se não estiver segurando, a corda volta suavemente para o repouso
            pullPoint.localPosition = Vector3.Lerp(pullPoint.localPosition, StringStartPoint.localPosition, Time.deltaTime * 20f);
            currentPullAmount = 0;
        }

        UpdateStringVisuals();
    }

    public void UpdateStringPosition(Vector3 handWorldPos) 
    {
        // converte a posicao da mao de world para local
        Vector3 localHandPos = pullPoint.InverseTransformPoint(handWorldPos);

        //capa o movimento da mao impedindo que ultrapasse o ponto de repouso e o ponto maximo e impede de ir para os lados
        float stringLimit = Mathf.Clamp(localHandPos.z, -maxPullDistance, 0);

        //atualiza o pullpoint da corda (apenas visual)
        pullPoint.localPosition = localHandPos; // new Vector3(0, 0, stringLimit);

        //calcula força
        currentPullAmount = Mathf.Abs(stringLimit) / maxPullDistance;
        
    }
    void UpdateStringVisuals()
    {
        // Desenha a linha entre as pontas e o ponto de puxada
        lineRenderer.SetPosition(0, topPoint.position);
        lineRenderer.SetPosition(1, StringStartPoint.position);
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

