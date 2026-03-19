using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Singleton service that owns the single shared <see cref="SilverValkyrieInput"/> instance.
/// Must be initialized before any consumer (Flipper, Plunger, PauseMenuUI, etc.) so it
/// carries <see cref="DefaultExecutionOrder"/> of -100.
///
/// Provides centralised binding override persistence via PlayerPrefs so that rebinding
/// changes survive sessions. <see cref="KeyBindingsUI"/> reads and writes through this
/// service exclusively.
/// </summary>
[DefaultExecutionOrder(-100)]
public class InputService : MonoBehaviour
{
    private const string OverridesKey = "SV_InputOverrides";

    public static InputService Instance { get; private set; }

    /// <summary>The single shared input wrapper — all consumers subscribe to actions here.</summary>
    public SilverValkyrieInput Input { get; private set; }

    /// <summary>Raw asset reference — used by KeyBindingsUI for full-map conflict scanning.</summary>
    public InputActionAsset Asset => Input.asset;

    /// <summary>Shortcut to the Pause action for PauseMenuUI.</summary>
    public InputAction Pause => Input.Gameplay.Pause;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        Input = new SilverValkyrieInput();
        LoadOverrides();
        Input.Enable();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Input.Disable();
            Input.Dispose();
            Instance = null;
        }
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Serialises all current binding overrides to PlayerPrefs.</summary>
    public void SaveOverrides()
    {
        PlayerPrefs.SetString(OverridesKey, Input.asset.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
    }

    /// <summary>Removes all binding overrides and clears the PlayerPrefs entry.</summary>
    public void ResetOverrides()
    {
        Input.asset.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(OverridesKey);
        PlayerPrefs.Save();
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private void LoadOverrides()
    {
        if (PlayerPrefs.HasKey(OverridesKey))
            Input.asset.LoadBindingOverridesFromJson(PlayerPrefs.GetString(OverridesKey));
    }
}
