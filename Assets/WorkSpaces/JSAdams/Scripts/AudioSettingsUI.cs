using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Audio settings overlay — four sliders (Master, Music, SFX, Ambiance) that write
/// decibel values into the Audio Mixer via exposed parameters.
///
/// Slider range: 0.0001–1.0 (linear) maps to −80 dB–0 dB (log scale).
/// Values persist across sessions via PlayerPrefs and are restored every Start().
///
/// SETUP: In the Audio Mixer window, right-click each group's Volume parameter and
/// choose "Expose to script". Rename the exposed parameters exactly:
///   "MasterVolume", "MusicVolume", "SFXVolume", "AmbianceVolume"
/// </summary>
public class AudioSettingsUI : MonoBehaviour
{
    // ── Mixer param names (must match what is exposed in the Audio Mixer) ─────

    private const string ParamMaster   = "MasterVolume";
    private const string ParamMusic    = "MusicVolume";
    private const string ParamSFX      = "SFXVolume";
    private const string ParamAmbiance = "AmbianceVolume";

    // ── PlayerPrefs keys ──────────────────────────────────────────────────────

    private const string KeyMaster   = "SV_Vol_Master";
    private const string KeyMusic    = "SV_Vol_Music";
    private const string KeySFX      = "SV_Vol_SFX";
    private const string KeyAmbiance = "SV_Vol_Ambiance";

    // ── Inspector fields ──────────────────────────────────────────────────────

    [SerializeField] private GameObject  panel;
    [SerializeField] private AudioMixer  mixer;

    [Header("Sliders")]
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private Slider sliderAmbiance;

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
        // Wire callbacks in code so Inspector wiring is not required.
        sliderMaster?.onValueChanged.AddListener(OnMasterChanged);
        sliderMusic?.onValueChanged.AddListener(OnMusicChanged);
        sliderSFX?.onValueChanged.AddListener(OnSFXChanged);
        sliderAmbiance?.onValueChanged.AddListener(OnAmbianceChanged);

        // Restore persisted levels on every session start.
        ApplyPersistedLevels();
    }

    private void OnDestroy()
    {
        sliderMaster?.onValueChanged.RemoveListener(OnMasterChanged);
        sliderMusic?.onValueChanged.RemoveListener(OnMusicChanged);
        sliderSFX?.onValueChanged.RemoveListener(OnSFXChanged);
        sliderAmbiance?.onValueChanged.RemoveListener(OnAmbianceChanged);
    }

    private void OnApplicationQuit()
    {
        // Safety save — covers the case where the game exits with the panel open.
        SaveValues();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Shows the audio settings panel, hiding the caller.</summary>
    public void Show(GameObject caller)
    {
        _caller = caller;
        if (_caller != null) _caller.SetActive(false);
        LoadAndApply();
        panel?.SetActive(true);
    }

    /// <summary>Saves current slider values and restores the caller panel.</summary>
    public void OnClose()
    {
        SaveValues();
        panel?.SetActive(false);
        if (_caller != null) _caller.SetActive(true);
        _caller = null;
    }

    /// <summary>Saves values and tears down the panel without restoring the caller. Used when the entire pause stack is force-dismissed.</summary>
    public void ForceClose()
    {
        SaveValues();
        panel?.SetActive(false);
        _caller = null;
    }

    // ── Slider callbacks ──────────────────────────────────────────────────────

    /// <summary>Called by sliderMaster.onValueChanged.</summary>
    public void OnMasterChanged(float value)   { if (!_initialising) ApplyToMixer(ParamMaster,   value); }

    /// <summary>Called by sliderMusic.onValueChanged.</summary>
    public void OnMusicChanged(float value)    { if (!_initialising) ApplyToMixer(ParamMusic,    value); }

    /// <summary>Called by sliderSFX.onValueChanged.</summary>
    public void OnSFXChanged(float value)      { if (!_initialising) ApplyToMixer(ParamSFX,      value); }

    /// <summary>Called by sliderAmbiance.onValueChanged.</summary>
    public void OnAmbianceChanged(float value) { if (!_initialising) ApplyToMixer(ParamAmbiance, value); }

    // ── Private ───────────────────────────────────────────────────────────────

    /// <summary>Applies persisted PlayerPrefs values straight to the mixer (no slider involvement).</summary>
    private void ApplyPersistedLevels()
    {
        ApplyToMixer(ParamMaster,   PlayerPrefs.GetFloat(KeyMaster,   1f));
        ApplyToMixer(ParamMusic,    PlayerPrefs.GetFloat(KeyMusic,    1f));
        ApplyToMixer(ParamSFX,      PlayerPrefs.GetFloat(KeySFX,      1f));
        ApplyToMixer(ParamAmbiance, PlayerPrefs.GetFloat(KeyAmbiance, 1f));
    }

    private void LoadAndApply()
    {
        _initialising = true;
        InitSlider(sliderMaster,   KeyMaster,   ParamMaster);
        InitSlider(sliderMusic,    KeyMusic,    ParamMusic);
        InitSlider(sliderSFX,      KeySFX,      ParamSFX);
        InitSlider(sliderAmbiance, KeyAmbiance, ParamAmbiance);
        _initialising = false;
    }

    private void InitSlider(Slider slider, string key, string param)
    {
        if (slider == null) return;
        float v = PlayerPrefs.GetFloat(key, 1f);
        slider.value = v;
        ApplyToMixer(param, v);
    }

    private void SaveValues()
    {
        if (sliderMaster   != null) PlayerPrefs.SetFloat(KeyMaster,   sliderMaster.value);
        if (sliderMusic    != null) PlayerPrefs.SetFloat(KeyMusic,    sliderMusic.value);
        if (sliderSFX      != null) PlayerPrefs.SetFloat(KeySFX,      sliderSFX.value);
        if (sliderAmbiance != null) PlayerPrefs.SetFloat(KeyAmbiance, sliderAmbiance.value);
        PlayerPrefs.Save();
    }

    /// <summary>Converts a linear 0–1 value to dB and sets the mixer parameter.</summary>
    private void ApplyToMixer(string param, float linearValue)
    {
        if (mixer == null) return;
        float db = Mathf.Log10(Mathf.Max(linearValue, 0.0001f)) * 20f;
        if (!mixer.SetFloat(param, db))
            Debug.LogWarning($"[AudioSettingsUI] Could not set mixer param '{param}'. Is it exposed in the Audio Mixer?");
    }
}

