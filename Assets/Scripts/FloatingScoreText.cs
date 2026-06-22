using UnityEngine;
using TMPro;

// Script que fica no prefab do texto flutuante "+100" que aparece no target destruído
public class FloatingScoreText : MonoBehaviour
{
    [Header("Configurações de Animação")]
    // Velocidade com que o texto sobe
    public float floatSpeed = 1f;

    // Tempo total até o texto desaparecer e ser destruído
    public float lifetime = 1f;

    [Header("Câmera (opcional)")]
    [Tooltip("Deixe vazio para usar Camera.main automaticamente. " +
             "Só preencha se a câmera do XR Origin não tiver a tag 'MainCamera'.")]
    public Camera targetCamera;

    [Header("Referência")]
    // TextMeshPro que mostra o valor — auto-detectado se não atribuído
    public TextMeshPro textMesh;

    private float elapsed = 0f;
    private Color startColor;
    private Transform cachedTransform;

    void Awake()
    {
        cachedTransform = transform;

        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshPro>();

        if (textMesh == null)
            Debug.LogError("[FloatingScoreText] ERRO: Nenhum TextMeshPro encontrado! " +
                           "O prefab precisa ter um componente TextMeshPro (3D), não TextMeshProUGUI.");

        // Se não foi atribuída manualmente no Inspector, tenta achar automaticamente
        if (targetCamera == null)
        {
            targetCamera = Camera.main;

            // Fallback extra — comum em VR a Main Camera não ter a tag "MainCamera"
            if (targetCamera == null)
                targetCamera = FindFirstObjectByType<Camera>();

            if (targetCamera == null)
                Debug.LogWarning("[FloatingScoreText] AVISO: Nenhuma câmera encontrada automaticamente. " +
                                 "O texto não vai rotacionar para encarar o player. " +
                                 "Arraste a câmera do XR Origin manualmente no campo 'Target Camera'.");
        }
    }

    // Chamado pelo Target logo após instanciar este prefab
    public void Setup(int points)
    {
        if (textMesh != null)
        {
            textMesh.text = "+" + points.ToString();
            startColor = textMesh.color;
        }

        Debug.Log("[FloatingScoreText] Texto configurado: +" + points);
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // Move o texto para cima suavemente
        cachedTransform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Faz o texto sempre olhar para a câmera (efeito billboard)
        if (targetCamera != null)
            cachedTransform.rotation = Quaternion.LookRotation(cachedTransform.position - targetCamera.transform.position);

        // Fade out gradual na segunda metade da vida do texto
        if (textMesh != null && elapsed > lifetime * 0.5f)
        {
            float fadeProgress = (elapsed - lifetime * 0.5f) / (lifetime * 0.5f);
            Color c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0f, fadeProgress);
            textMesh.color = c;
        }

        // Destrói o objeto ao fim do tempo de vida
        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}