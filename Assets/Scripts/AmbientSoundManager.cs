using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    [Header("Som Ambiente")]
    // AudioSource que vai tocar o som ambiente — auto-criado se não atribuído
    public AudioSource ambientSource;

    // Clipe de som ambiente (vento, floresta, multidão, etc)
    public AudioClip ambientClip;

    // Volume do som ambiente (0 a 1)
    [Range(0f, 1f)]
    public float ambientVolume = 0.4f;

    [Header("Espacialização")]
    // Se true, o som é 2D (mesmo volume em qualquer lugar da cena)
    // Se false, o som é 3D (varia com a distância da fonte)
    public bool is2DSound = true;

    void Start()
    {
        Debug.Log("[AmbientSoundManager] Iniciado.");
        SetupAmbientAudio();
        PlayAmbient();
    }

    void SetupAmbientAudio()
    {
        // Auto-detecta ou cria um AudioSource se não foi atribuído
        if (ambientSource == null)
        {
            ambientSource = GetComponent<AudioSource>();

            if (ambientSource == null)
            {
                ambientSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("[AmbientSoundManager] AudioSource criado automaticamente.");
            }
        }

        if (ambientClip == null)
        {
            Debug.LogWarning("[AmbientSoundManager] AVISO: ambientClip não atribuído! " +
                             "Arraste um arquivo de áudio no campo 'Ambient Clip'.");
            return;
        }

        ambientSource.clip = ambientClip;
        ambientSource.loop = true; // toca em loop infinito enquanto a cena existir
        ambientSource.volume = ambientVolume;
        ambientSource.playOnAwake = false;

        // 0 = som 2D (ignora posição), 1 = som 3D (totalmente espacial)
        ambientSource.spatialBlend = is2DSound ? 0f : 1f;

        Debug.Log("[AmbientSoundManager] Configurado: " + ambientClip.name +
                  " | Loop: true | 2D: " + is2DSound);
    }

    void PlayAmbient()
    {
        if (ambientSource == null || ambientClip == null)
        {
            Debug.LogWarning("[AmbientSoundManager] Som ambiente não pôde ser iniciado — " +
                             "source ou clip ausente.");
            return;
        }

        ambientSource.Play();
        Debug.Log("[AmbientSoundManager] Som ambiente iniciado.");
    }
}