using UnityEngine;

/// <summary>
/// Intermediate options panel. Sits between the pause menu and the three
/// settings sub-panels (Audio, Video, Controls). Uses the caller-swap pattern
/// so each sub-panel knows how to return here when closed.
/// </summary>
public class OptionsMenuUI : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────────────

    [SerializeField] private GameObject      panel;

    [Header("Sub-panels")]
    [SerializeField] private AudioSettingsUI audioSettings;
    [SerializeField] private VideoSettingsUI videoSettings;
    [SerializeField] private KeyBindingsUI   keyBindings;

    // ── State ─────────────────────────────────────────────────────────────────

    private GameObject _caller;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Shows the options menu, hiding the calling panel (e.g. pause menu).</summary>
    public void Show(GameObject caller)
    {
        _caller = caller;
        if (_caller != null) _caller.SetActive(false);
        panel?.SetActive(true);
    }

    /// <summary>Hides the options menu and restores the caller.</summary>
    public void OnClose()
    {
        panel?.SetActive(false);
        if (_caller != null) _caller.SetActive(true);
        _caller = null;
    }

    /// <summary>Opens the Audio Settings sub-panel.</summary>
    public void OnAudio() => audioSettings?.Show(panel);

    /// <summary>Opens the Video Settings sub-panel.</summary>
    public void OnVideo() => videoSettings?.Show(panel);

    /// <summary>Opens the Key Bindings sub-panel.</summary>
    public void OnControls() => keyBindings?.Show(panel);
}
