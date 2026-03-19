using System;
using UnityEngine;

/// <summary>
/// Tracks a time-decaying hit combo. Call <see cref="RegisterHit"/> on any scored shot
/// (orbit completions, spinner rips, bumper hits, etc.) to increment the counter and reset
/// the decay window. If no hit is registered within <see cref="comboDecayWindow"/> seconds,
/// the combo resets to zero automatically.
///
/// Call <see cref="ResetCombo"/> explicitly on ball drain to ensure immediate reset.
/// Subscribe to <see cref="OnComboChanged"/> for HUD and scoring-multiplier updates.
/// </summary>
[DisallowMultipleComponent]
public class ComboService : MonoBehaviour
{
    public static ComboService Instance { get; private set; }

    [Tooltip("Seconds of inactivity before the combo resets to zero.")]
    [SerializeField] private float comboDecayWindow = 2.5f;

    // ── Public state ──────────────────────────────────────────────────────────

    /// <summary>Current active combo streak count.</summary>
    public int CurrentCombo { get; private set; }

    // ── Events ────────────────────────────────────────────────────────────────

    /// <summary>Fires whenever the combo count changes. Parameter = new combo value (0 = reset).</summary>
    public static event Action<int> OnComboChanged;

    // ── Private ───────────────────────────────────────────────────────────────

    private float _decayTimer;
    private bool  _decayActive;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (!_decayActive || CurrentCombo <= 0) return;

        _decayTimer -= Time.deltaTime;
        if (_decayTimer > 0f) return;

        CurrentCombo = 0;
        _decayActive = false;
        OnComboChanged?.Invoke(0);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Increments the combo streak and resets the decay window.</summary>
    public void RegisterHit()
    {
        CurrentCombo++;
        _decayTimer  = comboDecayWindow;
        _decayActive = true;
        OnComboChanged?.Invoke(CurrentCombo);
    }

    /// <summary>Immediately zeroes the combo. Call on ball drain.</summary>
    public void ResetCombo()
    {
        if (CurrentCombo == 0) return;

        CurrentCombo = 0;
        _decayActive = false;
        OnComboChanged?.Invoke(0);
        Debug.Log("[ComboService] Combo reset on drain.");
    }
}
