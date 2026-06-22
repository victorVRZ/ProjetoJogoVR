using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    [Header("Configurações do Timer")]
    public float totalTime = 60f;

    [Header("Contagem Inicial")]
    // Quantos segundos de contagem regressiva antes do jogo começar (3, 2, 1...)
    public int countdownSeconds = 3;

    // Texto usado para mostrar a contagem (pode ser o mesmo timerText ou um separado)
    public TextMeshProUGUI countdownText;

    // Texto mostrado no último segundo antes de liberar o jogo (ex: "Vai!")
    public string startMessage = "Vai!";

    // Tempo que a mensagem de início fica na tela antes de desaparecer
    public float startMessageDuration = 0.5f;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("Spawners")]
    // Arraste aqui todos os TargetSpawner da cena (do player) — eles só começam
    // a spawnar depois que a contagem regressiva terminar
    public TargetSpawner[] targetSpawners;

    [Header("Bots")]
    // Arraste aqui todos os BotTargetSpawner da cena
    public BotTargetSpawner[] botTargetSpawners;

    // Arraste aqui todos os Bot da cena — eles não atiram durante a contagem
    public Bot[] bots;

    [Header("Leaderboard")]
    public LeaderboardUI leaderboardUI;

    [Header("Música")]
    public AudioSource musicSource;
    public AudioClip musicClip;
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;

    private float currentTime;
    private bool isRunning = false; // só começa a contar depois da contagem inicial

    void Start()
    {
        Debug.Log("[GameTimer] Iniciado. Aguardando contagem regressiva antes de começar.");

        if (timerText == null)
            Debug.LogError("[GameTimer] ERRO: timerText não atribuído!");

        if (leaderboardUI == null)
            Debug.LogError("[GameTimer] ERRO: leaderboardUI não atribuído!");

        if (countdownText == null)
            Debug.LogWarning("[GameTimer] AVISO: countdownText não atribuído. " +
                             "A contagem 3,2,1 não será exibida visualmente.");

        currentTime = totalTime;
        UpdateTimerUI();

        SetupMusic();

        // Garante que nada spawna ou atira durante a contagem.
        SetSpawnersEnabled(false);
        SetBotsEnabled(false);

        StartCoroutine(CountdownRoutine());
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            UpdateTimerUI();
            isRunning = false;
            Debug.Log("[GameTimer] Tempo esgotado!");

            StopMusic();
            TimeUp();
            return;
        }

        UpdateTimerUI();
    }

    // -------------------------------------------------------------------------
    // CONTAGEM REGRESSIVA INICIAL
    // -------------------------------------------------------------------------

    private IEnumerator CountdownRoutine()
    {
        // Esconde o timer normal durante a contagem
        if (timerText != null)
            timerText.gameObject.SetActive(false);

        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        for (int i = countdownSeconds; i > 0; i--)
        {
            if (countdownText != null)
                countdownText.text = i.ToString();

            Debug.Log("[GameTimer] Contagem: " + i);

            yield return new WaitForSeconds(1f);
        }

        if (countdownText != null)
            countdownText.text = startMessage;

        yield return new WaitForSeconds(startMessageDuration);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        // Mostra o timer normal de volta, já com o jogo liberado
        if (timerText != null)
            timerText.gameObject.SetActive(true);

        SetSpawnersEnabled(true);
        SetBotsEnabled(true);

        PlayMusic();
        isRunning = true;
    }

    // -------------------------------------------------------------------------
    // CONTROLE DOS SPAWNERS
    // -------------------------------------------------------------------------

    private void SetSpawnersEnabled(bool enabled)
    {
        if (targetSpawners == null || targetSpawners.Length == 0)
        {
            Debug.LogWarning("[GameTimer] AVISO: Nenhum TargetSpawner atribuído. " +
                             "Os spawners do player vão continuar funcionando normalmente.");
        }
        else
        {
            foreach (TargetSpawner spawner in targetSpawners)
            {
                if (spawner != null)
                    spawner.SetSpawningEnabled(enabled);
            }
        }

        if (botTargetSpawners == null || botTargetSpawners.Length == 0)
        {
            Debug.LogWarning("[GameTimer] AVISO: Nenhum BotTargetSpawner atribuído. " +
                             "Os spawners dos bots vão continuar funcionando normalmente.");
        }
        else
        {
            foreach (BotTargetSpawner spawner in botTargetSpawners)
            {
                if (spawner != null)
                    spawner.SetSpawningEnabled(enabled);
            }
        }

        Debug.Log("[GameTimer] Spawners " + (enabled ? "LIBERADOS" : "BLOQUEADOS") + ".");
    }

    private void SetBotsEnabled(bool enabled)
    {
        if (bots == null || bots.Length == 0)
        {
            Debug.LogWarning("[GameTimer] AVISO: Nenhum Bot atribuído no array 'Bots'. " +
                             "Os bots vão continuar atirando normalmente, sem esperar a contagem.");
            return;
        }

        foreach (Bot bot in bots)
        {
            if (bot == null) continue;

            if (enabled)
                bot.ResumeBot();
            else
                bot.StopBot();
        }

        Debug.Log("[GameTimer] Bots " + (enabled ? "LIBERADOS" : "BLOQUEADOS") + ".");
    }

    // -------------------------------------------------------------------------
    // TIMER
    // -------------------------------------------------------------------------

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerText.color = currentTime <= 10f ? Color.red : Color.white;
    }

    void TimeUp()
    {
        isRunning = false;

        if (leaderboardUI != null)
            leaderboardUI.ShowLeaderboard();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetRunning(bool running)
    {
        isRunning = running;
        Debug.Log("[GameTimer] Timer " + (running ? "retomado." : "pausado."));

        if (musicSource != null)
        {
            if (running) musicSource.UnPause();
            else musicSource.Pause();
        }
    }

    public float GetCurrentTime() => currentTime;

    // -------------------------------------------------------------------------
    // MÚSICA
    // -------------------------------------------------------------------------

    void SetupMusic()
    {
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("[GameTimer] AudioSource criado automaticamente.");
            }
        }

        if (musicClip == null)
        {
            Debug.LogWarning("[GameTimer] AVISO: musicClip não atribuído!");
            return;
        }

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;

        Debug.Log("[GameTimer] Música configurada: " + musicClip.name);
    }

    void PlayMusic()
    {
        if (musicSource == null || musicClip == null)
        {
            Debug.LogWarning("[GameTimer] Música não pôde ser iniciada.");
            return;
        }

        musicSource.Play();
        Debug.Log("[GameTimer] Música iniciada.");
    }

    void StopMusic()
    {
        if (musicSource == null) return;

        musicSource.Stop();
        Debug.Log("[GameTimer] Música parada.");
    }
}