using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Painéis do Menu")]
    // Painel principal: Jogar / Tutorial / Sair
    public GameObject mainPanel;

    // Painel de seleção de dificuldade: Fácil / Médio / Difícil / Voltar
    public GameObject difficultyPanel;

    // Painel de tutorial: instruções + botão Voltar
    public GameObject tutorialPanel;

    [Header("Configuração de Cena")]
    [Tooltip("Nome exato da cena do jogo (precisa estar em Build Profiles/Build Settings)")]
    public string gameSceneName = "Gameplay";

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        Debug.Log("[MainMenuController] Menu iniciado.");

        if (mainPanel == null)
            Debug.LogError("[MainMenuController] ERRO: mainPanel não atribuído!");

        if (difficultyPanel == null)
            Debug.LogError("[MainMenuController] ERRO: difficultyPanel não atribuído!");

        if (tutorialPanel == null)
            Debug.LogError("[MainMenuController] ERRO: tutorialPanel não atribuído!");

        // Garante que só o painel principal está visível ao abrir o menu
        ShowMainPanel();
    }

    // -------------------------------------------------------------------------
    // NAVEGAÇÃO ENTRE PAINÉIS — chamados pelos botões (OnClick)
    // -------------------------------------------------------------------------

    public void ShowMainPanel()
    {
        Debug.Log("[MainMenuController] Exibindo painel principal.");
        SetActivePanel(mainPanel);
    }

    // Chamado pelo botão "Jogar"
    public void OnPlayButtonPressed()
    {
        Debug.Log("[MainMenuController] Botão Jogar pressionado. Exibindo seleção de dificuldade.");
        SetActivePanel(difficultyPanel);
    }

    // Chamado pelo botão "Tutorial"
    public void OnTutorialButtonPressed()
    {
        Debug.Log("[MainMenuController] Botão Tutorial pressionado.");
        SetActivePanel(tutorialPanel);
    }

    // Chamado pelo botão "Sair"
    public void OnQuitButtonPressed()
    {
        Debug.Log("[MainMenuController] Botão Sair pressionado. Encerrando aplicação.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Chamado pelo botão "Voltar" dentro do painel de dificuldade ou tutorial
    public void OnBackButtonPressed()
    {
        Debug.Log("[MainMenuController] Voltando ao painel principal.");
        ShowMainPanel();
    }

    // -------------------------------------------------------------------------
    // SELEÇÃO DE DIFICULDADE — chamados pelos 3 botões do painel de dificuldade
    // -------------------------------------------------------------------------

    public void OnEasySelected()
    {
        Debug.Log("[MainMenuController] Dificuldade selecionada: FÁCIL");
        StartGame(Difficulty.Easy);
    }

    public void OnMediumSelected()
    {
        Debug.Log("[MainMenuController] Dificuldade selecionada: MÉDIO");
        StartGame(Difficulty.Medium);
    }

    public void OnHardSelected()
    {
        Debug.Log("[MainMenuController] Dificuldade selecionada: DIFÍCIL");
        StartGame(Difficulty.Hard);
    }

    // -------------------------------------------------------------------------
    // INÍCIO DO JOGO
    // -------------------------------------------------------------------------

    private void StartGame(Difficulty difficulty)
    {
        if (DifficultyManager.Instance == null)
        {
            Debug.LogError("[MainMenuController] ERRO: DifficultyManager não encontrado na cena! " +
                           "Certifique-se de que existe um GameObject com o script DifficultyManager " +
                           "na cena do MainMenu.");
            return;
        }

        DifficultyManager.Instance.SetDifficulty(difficulty);

        Debug.Log("[MainMenuController] Carregando cena do jogo: " + gameSceneName);

        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("[MainMenuController] ERRO: gameSceneName está vazio! " +
                           "Defina o nome exato da cena no Inspector.");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    // -------------------------------------------------------------------------
    // HELPER PRIVADO
    // -------------------------------------------------------------------------

    private void SetActivePanel(GameObject panelToShow)
    {
        if (mainPanel != null) mainPanel.SetActive(panelToShow == mainPanel);
        if (difficultyPanel != null) difficultyPanel.SetActive(panelToShow == difficultyPanel);
        if (tutorialPanel != null) tutorialPanel.SetActive(panelToShow == tutorialPanel);
    }
}