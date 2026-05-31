using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // VARIÁVEIS PÚBLICAS — configuráveis pelo Inspector
    // -------------------------------------------------------------------------

    [Header("Configurações do Timer")]

    // Tempo total em segundos (ex: 60 = 1 minuto)
    public float totalTime = 60f;

    [Header("UI")]

    // Texto que exibe o tempo na tela
    public TextMeshProUGUI timerText;

    // -------------------------------------------------------------------------
    // VARIÁVEIS PRIVADAS
    // -------------------------------------------------------------------------

    // Tempo restante atual
    private float currentTime;

    // Controla se o timer está rodando ou pausado
    private bool isRunning = true;

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        Debug.Log("[GameTimer] Timer iniciado com " + totalTime + " segundos.");

        // Verifica se o texto foi atribuído no Inspector
        if (timerText == null)
        {
            Debug.LogError("[GameTimer] ERRO: timerText não foi atribuído no Inspector! " +
                           "Arraste o TextMeshPro do timer no campo 'Timer Text'.");
        }

        // Inicializa o tempo atual com o total configurado
        currentTime = totalTime;

        // Atualiza a UI logo no início para não aparecer zerado por 1 frame
        UpdateTimerUI();
    }

    void Update()
    {
        // Não faz nada se o timer estiver pausado ou já zerado
        if (!isRunning) return;

        // Diminui o tempo usando o deltaTime (tempo entre frames)
        currentTime -= Time.deltaTime;

        Debug.Log("[GameTimer] Tempo restante: " + currentTime.ToString("F1") + "s");

        // Garante que o tempo não passe de zero
        if (currentTime <= 0f)
        {
            currentTime = 0f;

            // Atualiza a UI para mostrar 0 antes de reiniciar
            UpdateTimerUI();

            // Para o timer para não chamar TimeUp múltiplas vezes
            isRunning = false;

            Debug.Log("[GameTimer] Tempo esgotado! Chamando TimeUp()...");

            TimeUp();
            return;
        }

        // Atualiza o texto na tela a cada frame
        UpdateTimerUI();
    }

    // -------------------------------------------------------------------------
    // MÉTODOS PRIVADOS
    // -------------------------------------------------------------------------

    // Formata e atualiza o texto do timer na UI
    void UpdateTimerUI()
    {
        if (timerText == null) return;

        // Converte segundos para formato MM:SS
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        // Exibe no formato "01:30"
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Muda a cor para vermelho nos últimos 10 segundos
        if (currentTime <= 10f)
        {
            timerText.color = Color.red;
            Debug.Log("[GameTimer] AVISO: Menos de 10 segundos restantes!");
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    // Chamado quando o tempo chega a zero
    void TimeUp()
    {
        Debug.Log("[GameTimer] Reiniciando cena: " + SceneManager.GetActiveScene().name);

        // Reinicia a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // -------------------------------------------------------------------------
    // MÉTODOS PÚBLICOS — podem ser chamados por outros scripts
    // -------------------------------------------------------------------------

    // Pausa ou retoma o timer externamente
    public void SetRunning(bool running)
    {
        isRunning = running;
        Debug.Log("[GameTimer] Timer " + (running ? "retomado." : "pausado."));
    }

    // Retorna o tempo restante atual (útil para outros scripts)
    public float GetCurrentTime() => currentTime;
}