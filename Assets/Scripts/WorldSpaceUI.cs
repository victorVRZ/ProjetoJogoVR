using UnityEngine;

public class WorldSpaceUI : MonoBehaviour
{
    [Header("Referências")]
    public Transform playerCamera; // arrasta o XR Camera aqui

    [Header("Posição relativa ao player")]
    public Vector3 offset = new Vector3(0.4f, -0.2f, 0.6f); // direita, baixo, frente
    public float followSpeed = 5f;
    public bool facePlayer = true;

    void LateUpdate()
    {
        if (playerCamera == null) return;

        // Calcula posição alvo no espaço local da câmera
        Vector3 targetPosition = playerCamera.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Faz o monitor sempre olhar para o player
        if (facePlayer)
        {
            transform.LookAt(playerCamera);
            transform.Rotate(0, 180f, 0); // inverte para ficar legível
        }
    }
}
