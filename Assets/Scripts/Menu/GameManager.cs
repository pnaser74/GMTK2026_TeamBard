using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
    Beginning,
    Loading,
    Gameplay,
    Paused
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Wiring")]
    [SerializeField] private ScreenRouter _router;
    [SerializeField] private CheckpointTableSO _checkpoints;

    [Header("Screens")]
    [SerializeField] private MenuScreen _titleScreen;
    [SerializeField] private PauseScreen _pauseScreen;
    [SerializeField] private MenuScreen _introScreen;
    [SerializeField] private MenuScreen _endingScreen;
    [SerializeField] private MenuScreen _creditsScreen;

    [Header("Scenes")]
    [SerializeField] private string _mainMenuSceneName = "MainMenu";

    public GameMode Mode { get; private set; } = GameMode.Beginning;

    public int PendingCheckpoint { get; private set; }

    public CheckpointTableSO Checkpoints => _checkpoints;
    public string MainMenuSceneName => _mainMenuSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //PlayerPrefs needs no FMOD, so settings are loaded before anything can play.
        SettingsService.Load();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == _mainMenuSceneName)
        {
            EnterBeginning();
            return;
        }

        // pressed Play directly in a level scene
        EnterGameplay();
    }

    private void Update()
    {
        // skip if pause was already pressed
        if (!GameInput.PauseAction.WasPerformedThisFrame())
            return;

        if (Mode == GameMode.Gameplay)
        {
            Pause();
            return;
        }

        if (Mode == GameMode.Paused && _router.Top == _pauseScreen)
            Resume();
    }

    public void OnCancelOutsideMenus()
    {
        if (Mode == GameMode.Gameplay)
            Pause();
    }
    
    public void EnterBeginning()
    {
        Mode = GameMode.Beginning;
        Time.timeScale = 1f;
        GameInput.SetGameplayEnabled(false);
        _router.ResetTo(_titleScreen);
    }

    public void EnterGameplay()
    {
        Mode = GameMode.Gameplay;
        Time.timeScale = 1f;
        _router.ClearAll();
        GameInput.SetGameplayEnabled(true);
    }

    public void Pause()
    {
        if (Mode != GameMode.Gameplay)
            return;

        Mode = GameMode.Paused;
        GameInput.SetGameplayEnabled(false);
        Time.timeScale = 0f;
        _router.Push(_pauseScreen);
    }

    public void Resume()
    {
        if (Mode != GameMode.Paused)
            return;

        _router.ClearAll();
        Time.timeScale = 1f;
        GameInput.SetGameplayEnabled(true);
        Mode = GameMode.Gameplay;
    }

    // ---- menu verbs ----

    public void StartNewGame()
    {
        //no save is written here - the first CheckpointMarker the player reaches does it.
        SaveService.DeleteSave();
        PendingCheckpoint = SaveService.IntroCheckpoint;

        //the intro is a stub screen for now; it calls EnterCheckpoint when dismissed.
        if (_introScreen != null)
        {
            _router.Push(_introScreen);
            return;
        }

        LoadCheckpoint(SaveService.IntroCheckpoint);
    }

    public void ContinueGame()
    {
        if (!SaveService.HasSave)
            return;

        if (SaveService.Completed)
        {
            ShowEnding();
            return;
        }

        LoadCheckpoint(SaveService.Checkpoint);
    }

    public void LoadCheckpoint(int globalCheckpoint)
    {
        if (_checkpoints == null)
        {
            Debug.LogError("[GameManager] no checkpoints assigned - cannot start game.");
            return;
        }

        var sceneName = _checkpoints.IntroSceneName;
        PendingCheckpoint = globalCheckpoint;

        if (globalCheckpoint != SaveService.IntroCheckpoint)
        {
            var entry = _checkpoints.Find(globalCheckpoint);
            if (entry == null || string.IsNullOrEmpty(entry.sceneName))
            {
                Debug.LogWarning($"[GameManager] checkpoint {globalCheckpoint} is not in the table - falling back to the intro scene.");
                PendingCheckpoint = SaveService.IntroCheckpoint;
            }
            else
            {
                sceneName = entry.sceneName;
            }
        }

        StartCoroutine(LoadSceneThenPlay(sceneName));
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        GameInput.SetGameplayEnabled(false);
        Mode = GameMode.Loading;
        _router.ClearAll();
        StartCoroutine(LoadSceneThenFrontEnd(_mainMenuSceneName));
    }

    public void ShowCredits()
    {
        if (_creditsScreen != null)
            _router.Push(_creditsScreen);
    }

    public void ShowEnding()
    {
        if (_endingScreen != null)
            _router.Push(_endingScreen);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        //Application.Quit is a no-op in a browser
        Debug.Log("[GameManager] quit requested, ignored on WebGL.");
#else
        Application.Quit();
#endif
    }

    public static bool CanQuit
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return false;
#else
            return true;
#endif
        }
    }

    // ---- scene loading ----

    private IEnumerator LoadSceneThenPlay(string sceneName)
    {
        Mode = GameMode.Loading;
        GameInput.SetGameplayEnabled(false);
        Time.timeScale = 1f;
        _router.ClearAll();

        yield return SceneManager.LoadSceneAsync(sceneName);
        
        EnterGameplay();
    }

    private IEnumerator LoadSceneThenFrontEnd(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
        EnterBeginning();
    }

    public void ConsumePendingCheckpoint()
    {
        PendingCheckpoint = SaveService.IntroCheckpoint;
    }
}
