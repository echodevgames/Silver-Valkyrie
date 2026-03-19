using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the pause menu panel.
/// Assign <see cref="pauseAction"/> to a Pause InputAction for full multi-device support.
/// Without an assigned action, falls back to Escape / P / Gamepad Start polling.
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject         panel;
    [SerializeField] private HighScoreBoardUI   highScoreBoard;

    [Tooltip("Optional — assign a Pause InputAction from your InputActions asset for " +
             "full controller / rebinding support. Leave empty to use the keyboard/gamepad fallback.")]
    [SerializeField] private InputActionReference pauseAction;

    private bool _isPaused;

    private const float TimeScalePlaying = 1f;
    private const float TimeScalePaused  = 0f;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void OnEnable()
    {
        if (pauseAction == null) return;
        pauseAction.action.performed += OnPauseActionPerformed;
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        if (pauseAction == null) return;
        pauseAction.action.performed -= OnPauseActionPerformed;
    }

    private void Update()
    {
        // When a proper InputAction is assigned, skip the polling fallback.
        if (pauseAction != null) return;

        var keyboard = Keyboard.current;
        var gamepad  = Gamepad.current;

        bool triggered =
            (keyboard != null && (keyboard.escapeKey.wasPressedThisFrame || keyboard.pKey.wasPressedThisFrame)) ||
            (gamepad  != null && gamepad.startButton.wasPressedThisFrame);

        if (triggered) TryTogglePause();
    }

    private void OnPauseActionPerformed(InputAction.CallbackContext ctx) => TryTogglePause();

    /// <summary>
    /// Guards against opening during a game-over freeze, then calls <see cref="TogglePause"/>.
    /// </summary>
    private void TryTogglePause()
    {
        if (Time.timeScale == TimeScalePaused && !_isPaused) return;
        TogglePause();
    }

    /// <summary>Freezes or unfreezes time and audio, and shows or hides the panel.</summary>
    public void TogglePause()
    {
        _isPaused            = !_isPaused;
        Time.timeScale       = _isPaused ? TimeScalePaused  : TimeScalePlaying;
        AudioListener.pause  = _isPaused;
        panel?.SetActive(_isPaused);
    }

    // ── Button callbacks — wired via Inspector PersistentCalls ────────────────

    /// <summary>Closes the pause menu and resumes play.</summary>
    public void OnResume()
    {
        Debug.Log("[PauseMenu] Resume");
        if (_isPaused) TogglePause();
    }

    /// <summary>Resets time, audio, and panel state, then reloads the active scene via GameDirector.</summary>
    public void OnRestart()
    {
        Debug.Log("[PauseMenu] Restart");

        // Full state reset before reload — SceneManager.LoadScene does not reset these automatically.
        _isPaused            = false;
        Time.timeScale       = 1f;
        AudioListener.pause  = false;
        panel?.SetActive(false);

        GameDirector.Instance?.RestartGame();
    }

    /// <summary>Stub — open options / settings screen when implementing.</summary>
    public void OnOptions()
    {
        Debug.Log("[PauseMenu] Options — stub");
    }

    /// <summary>Opens the high score board overlay.</summary>
    public void OnHighScores()
    {
        Debug.Log("[PauseMenu] High Scores");
        highScoreBoard?.Show();
    }

    /// <summary>Stub — open credits screen when implementing.</summary>
    public void OnCredits()
    {
        Debug.Log("[PauseMenu] Credits — stub");
    }

    /// <summary>Exits play mode in the Editor; quits the application in builds.</summary>
    public void OnQuit()
    {
        Debug.Log("[PauseMenu] Quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
