using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSpaceRestartButton : MonoBehaviour
{
    [Header("Referências")]
    // Renderer do botão para feedback visual
    public Renderer buttonRenderer;

    [Header("Cores do Botão")]
    // Cor normal do botão
    public Color normalColor = Color.green;

    // Cor quando a mão está perto
    public Color hoverColor = Color.yellow;

    // Cor ao pressionar
    public Color pressColor = Color.red;

    [Header("Configurações")]
    // Tag da mão do player
    public string handTag = "Player";

    // Tempo que a mão precisa ficar no botão para ativar (em segundos)
    public float activationDelay = 1f;

    // Nome da propriedade de cor no shader
    public string colorPropertyName = "_BaseColor";

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS
    // -------------------------------------------------------------------------

    // Contador de quanto tempo a mão está no botão
    private float handTimer = 0f;

    // Se a mão está atualmente no botão
    private bool handInside = false;

    // Se o botão já foi ativado (evita ativar duas vezes)
    private bool activated = false;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        Debug.Log("[RestartButton] Botão físico iniciado.");

        // Começa desativado — só aparece com a leaderboard
        gameObject.SetActive(false);

        if (buttonRenderer == null)
            buttonRenderer = GetComponentInChildren<Renderer>();

        if (buttonRenderer == null)
            Debug.LogError("[RestartButton] ERRO: Renderer não encontrado! " +
                           "Arraste o Renderer do botão no campo 'Button Renderer'.");

        SetColor(normalColor);
    }

    void Update()
    {
        if (!handInside || activated) return;

        // Acumula o tempo que a mão está no botão
        handTimer += Time.unscaledDeltaTime;

        // Feedback visual de progresso — interpola entre hover e press
        float progress = handTimer / activationDelay;
        Color currentColor = Color.Lerp(hoverColor, pressColor, progress);
        SetColor(currentColor);

        Debug.Log("[RestartButton] Progresso de ativação: " + (progress * 100f).ToString("F0") + "%");

        // Ativa ao completar o tempo
        if (handTimer >= activationDelay)
        {
            Activate();
        }
    }

    // -------------------------------------------------------------------------
    // DETECÇÃO DE MÃO
    // -------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[RestartButton] TRIGGER ATIVADO por: " + other.gameObject.name +
              " | Tag: " + other.tag + " | Layer: " + other.gameObject.layer);

        if (activated) return;

        if (other.CompareTag(handTag))
        {
            handInside = true;
            handTimer = 0f;
            SetColor(hoverColor);
            Debug.Log("[RestartButton] Mão detectada!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(handTag))
        {
            handInside = false;
            handTimer = 0f;
            SetColor(normalColor);
            Debug.Log("[RestartButton] Mão saiu do botão. Timer resetado.");
        }
    }

    // -------------------------------------------------------------------------
    // ATIVAÇÃO
    // -------------------------------------------------------------------------

    private void Activate()
    {
        if (activated) return;
        activated = true;

        SetColor(pressColor);
        Debug.Log("[RestartButton] Botão ativado! Reiniciando cena...");

        // Descongela o tempo antes de reiniciar
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // -------------------------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------------------------

    private void SetColor(Color color)
    {
        if (buttonRenderer == null) return;
        if (buttonRenderer.material == null) return;

        if (buttonRenderer.material.HasProperty(colorPropertyName))
            buttonRenderer.material.SetColor(colorPropertyName, color);
    }
}