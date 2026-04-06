using UnityEditor;
using UnityEngine;

public class BowLogic : MonoBehaviour
{
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

