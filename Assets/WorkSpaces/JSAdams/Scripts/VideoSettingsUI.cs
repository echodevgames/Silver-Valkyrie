using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Video settings overlay — fullscreen toggle, VSync toggle, and quality-level dropdown.
/// Settings apply immediately and persist across sessions via PlayerPrefs.
/// </summary>
public class VideoSettingsUI : MonoBehaviour
{
    // ── PlayerPrefs keys ──────────────────────────────────────────────────────

    private const string KeyFullscreen = "SV_Fullscreen";
    private const string KeyVSync      = "SV_VSync";
    private const string KeyQuality    = "SV_Quality";

    // ── Inspector fields ──────────────────────────────────────────────────────

    [SerializeField] private GameObject    panel;

    [Header("Controls")]
    [SerializeField] private Toggle        fullscreenToggle;
    [SerializeField] private Toggle        vsyncToggle;
    [SerializeField] private TMP_Dropdown  qualityDropdown;

    // ── State ─────────────────────────────────────────────────────────────────

    private GameObject _caller;
    private bool       _initialising;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    private void Start()
    {
        fullscreenToggle?.onValueChanged.AddListener(OnFullscreenChanged);
        vsyncToggle?.onValueChanged.AddListener(OnVSyncChanged);
        qualityDropdown?.onValueChanged.AddListener(OnQualityChanged);
    }

    private void OnDestroy()
    {
        fullscreenToggle?.onValueChanged.RemoveListener(OnFullscreenChanged);
        vsyncToggle?.onValueChanged.RemoveListener(OnVSyncChanged);
        qualityDropdown?.onValueChanged.RemoveListener(OnQualityChanged);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Shows the video settings panel, hiding the caller.</summary>
    public void Show(GameObject caller)
    {
        _caller = caller;
        if (_caller != null) _caller.SetActive(false);
        LoadValues();
        panel?.SetActive(true);
    }

    /// <summary>Saves settings and restores the caller panel.</summary>
    public void OnClose()
    {
        SaveValues();
        panel?.SetActive(false);
        if (_caller != null) _caller.SetActive(true);
        _caller = null;
    }

    /// <summary>Saves settings and tears down the panel without restoring the caller. Used when the entire pause stack is force-dismissed.</summary>
    public void ForceClose()
    {
        SaveValues();
        panel?.SetActive(false);
        _caller = null;
    }

    // ── Toggle / Dropdown callbacks ───────────────────────────────────────────

    /// <summary>Applies fullscreen state immediately.</summary>
    public void OnFullscreenChanged(bool value)
    {
        if (_initialising) return;
        Screen.fullScreen = value;
    }

    /// <summary>Applies VSync state immediately.</summary>
    public void OnVSyncChanged(bool value)
    {
        if (_initialising) return;
        QualitySettings.vSyncCount = value ? 1 : 0;
    }

    /// <summary>Applies quality level immediately.</summary>
    public void OnQualityChanged(int index)
    {
        if (_initialising) return;
        QualitySettings.SetQualityLevel(index, applyExpensiveChanges: true);
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private void LoadValues()
    {
        _initialising = true;

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = PlayerPrefs.GetInt(KeyFullscreen, Screen.fullScreen ? 1 : 0) == 1;

        if (vsyncToggle != null)
            vsyncToggle.isOn = PlayerPrefs.GetInt(KeyVSync, QualitySettings.vSyncCount > 0 ? 1 : 0) == 1;

        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
            qualityDropdown.value = PlayerPrefs.GetInt(KeyQuality, QualitySettings.GetQualityLevel());
            qualityDropdown.RefreshShownValue();
        }

        _initialising = false;
    }

    private void SaveValues()
    {
        PlayerPrefs.SetInt(KeyFullscreen, Screen.fullScreen ? 1 : 0);
        PlayerPrefs.SetInt(KeyVSync,      QualitySettings.vSyncCount > 0 ? 1 : 0);
        PlayerPrefs.SetInt(KeyQuality,    QualitySettings.GetQualityLevel());
        PlayerPrefs.Save();
    }
}
