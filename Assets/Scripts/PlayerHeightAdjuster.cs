using UnityEngine;
using TMPro;

public class PlayerHeightAdjuster : MonoBehaviour
{
    [Header("Referências")]
    // Arraste o "Camera Offset" do XR Origin aqui (objeto que controla a altura da câmera)
    public Transform cameraOffset;

    [Header("Configurações de Altura")]
    // Altura mínima permitida (offset em metros)
    public float minHeightOffset = -0.3f;

    // Altura máxima permitida (offset em metros)
    public float maxHeightOffset = 0.3f;

    // Quanto cada clique no botão sobe/desce (em metros)
    public float heightStep = 0.05f;

    [Header("UI (opcional)")]
    // Texto que mostra o valor atual do ajuste de altura
    public TextMeshProUGUI heightDisplayText;

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS
    // -------------------------------------------------------------------------

    // Offset atual aplicado (relativo à altura padrão do Tracking Origin)
    private float currentHeightOffset = 0f;

    // Posição Y original do Camera Offset antes de qualquer ajuste
    private float baseYPosition;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        Debug.Log("[PlayerHeightAdjuster] Iniciado.");

        if (cameraOffset == null)
        {
            Debug.LogError("[PlayerHeightAdjuster] ERRO: cameraOffset não atribuído! " +
                           "Arraste o GameObject 'Camera Offset' do XR Origin no Inspector.");
            return;
        }

        // Salva a posição Y original como base
        baseYPosition = cameraOffset.localPosition.y;

        Debug.Log("[PlayerHeightAdjuster] Posição Y base: " + baseYPosition);

        UpdateHeightDisplay();
    }

    // -------------------------------------------------------------------------
    // MÉTODOS PÚBLICOS — chamados pelos botões do menu (onClick)
    // -------------------------------------------------------------------------

    // Aumenta a altura do player
    public void IncreaseHeight()
    {
        SetHeightOffset(currentHeightOffset + heightStep);
        Debug.Log("[PlayerHeightAdjuster] Altura aumentada para offset: " + currentHeightOffset.ToString("F2"));
    }

    // Diminui a altura do player
    public void DecreaseHeight()
    {
        SetHeightOffset(currentHeightOffset - heightStep);
        Debug.Log("[PlayerHeightAdjuster] Altura diminuída para offset: " + currentHeightOffset.ToString("F2"));
    }

    // Reseta a altura para o padrão (offset 0)
    public void ResetHeight()
    {
        SetHeightOffset(0f);
        Debug.Log("[PlayerHeightAdjuster] Altura resetada para o padrão.");
    }

    // Permite definir a altura diretamente via Slider (UI.Slider.onValueChanged)
    public void SetHeightFromSlider(float sliderValue)
    {
        // Espera-se que o Slider tenha Min = minHeightOffset e Max = maxHeightOffset
        SetHeightOffset(sliderValue);
    }

    // -------------------------------------------------------------------------
    // MÉTODO PRIVADO PRINCIPAL
    // -------------------------------------------------------------------------

    private void SetHeightOffset(float newOffset)
    {
        if (cameraOffset == null)
        {
            Debug.LogError("[PlayerHeightAdjuster] ERRO: cameraOffset é null. Não é possível ajustar altura.");
            return;
        }

        // Garante que o offset fique dentro dos limites configurados
        currentHeightOffset = Mathf.Clamp(newOffset, minHeightOffset, maxHeightOffset);

        // Aplica o novo valor de Y mantendo X e Z originais
        Vector3 pos = cameraOffset.localPosition;
        pos.y = baseYPosition + currentHeightOffset;
        cameraOffset.localPosition = pos;

        Debug.Log("[PlayerHeightAdjuster] Nova posição Y do Camera Offset: " + pos.y.ToString("F3"));

        UpdateHeightDisplay();
    }

    private void UpdateHeightDisplay()
    {
        if (heightDisplayText == null) return;

        // Mostra o offset em centímetros para facilitar a leitura
        float offsetCm = currentHeightOffset * 100f;
        string sign = offsetCm >= 0 ? "+" : "";

        heightDisplayText.text = "Altura: " + sign + offsetCm.ToString("F0") + " cm";
    }
}