using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Configurações do Timer")]
    public float totalTime = 60f;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("Leaderboard")]
    // Arraste o LeaderboardUI aqui
    public LeaderboardUI leaderboardUI;

    [Header("Música")]
    // AudioSource que vai tocar a música — arraste ou deixa auto-detectar
    public AudioSource musicSource;

    // Clipe de música a ser tocado enquanto o timer corre
    public AudioClip musicClip;

    // Volume da música (0 a 1)
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;

    private float currentTime;
    private bool isRunning = true;

    void Start()
    {
        Debug.Log("[GameTimer] Timer iniciado com " + totalTime + " segundos.");

        if (timerText == null)
            Debug.LogError("[GameTimer] ERRO: timerText não atribuído!");

        if (leaderboardUI == null)
            Debug.LogError("[GameTimer] ERRO: leaderboardUI não atribuído! " +
                           "Arraste o LeaderboardUI no campo correspondente.");

        currentTime = totalTime;
        UpdateTimerUI();

        SetupMusic();
        PlayMusic();
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
        isRunning = false; // para o timer sem congelar a cena

        if (leaderboardUI != null)
            leaderboardUI.ShowLeaderboard();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetRunning(bool running)
    {
        isRunning = running;
        Debug.Log("[GameTimer] Timer " + (running ? "retomado." : "pausado."));

        // Pausa ou retoma a música junto com o timer
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
        // Se não foi atribuído no Inspector, tenta achar ou criar um AudioSource
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
            Debug.LogWarning("[GameTimer] AVISO: musicClip não atribuído! " +
                             "Arraste um arquivo de áudio no campo 'Music Clip'.");
            return;
        }

        musicSource.clip = musicClip;
        musicSource.loop = true; // garante que a música repete enquanto o timer corre
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;

        Debug.Log("[GameTimer] Música configurada: " + musicClip.name + " | Loop: true");
    }

    void PlayMusic()
    {
        if (musicSource == null || musicClip == null)
        {
            Debug.LogWarning("[GameTimer] Música não pôde ser iniciada — source ou clip ausente.");
            return;
        }

        musicSource.Play();
        Debug.Log("[GameTimer] Música iniciada.");
    }

    void StopMusic()
    {
        if (musicSource == null) return;

        musicSource.Stop();
        Debug.Log("[GameTimer] Música parada — tempo esgotado.");
    }
}