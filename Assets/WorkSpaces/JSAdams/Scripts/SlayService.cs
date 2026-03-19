using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the SLAY kill-chain system. Call <see cref="RegisterKill"/> each time an enemy dies.
///
/// The chain builds S→L→A→Y. If the decay window expires between kills, the chain steps back
/// exactly one letter and the timer restarts — the player is never penalised all the way to zero
/// in one miss, but sustained inactivity drains the chain fully.
///
/// When Y lights (count reaches 4) SLAY Mode activates for <see cref="slayModeDuration"/> seconds,
/// after which the chain resets to zero and normal rules resume.
///
/// Subscribe to <see cref="OnSlayCountChanged"/>, <see cref="OnSlayModeStarted"/>, and
/// <see cref="OnSlayModeEnded"/> for HUD updates and gameplay effects (fire VFX, chain ignition).
/// </summary>
[DisallowMultipleComponent]
public class SlayService : MonoBehaviour
{
    public static SlayService Instance { get; private set; }

    [Tooltip("Seconds allowed between kills before the chain steps back one letter.")]
    [SerializeField] private float decayWindow = 3f;

    [Tooltip("How long SLAY Mode lasts in seconds.")]
    [SerializeField] private float slayModeDuration = 10f;

    // ── Public state ──────────────────────────────────────────────────────────

    /// <summary>Current lit letter count: 0 = none, 1 = S, 2 = S+L, 3 = S+L+A, 4 = all (mode active).</summary>
    public int CurrentCount { get; private set; }

    /// <summary>True while SLAY Mode is running.</summary>
    public bool IsSlayModeActive { get; private set; }

    // ── Events ────────────────────────────────────────────────────────────────

    /// <summary>Fires whenever the kill-chain count changes. Parameter = new count (0–4).</summary>
    public static event Action<int> OnSlayCountChanged;

    /// <summary>Fires when all four letters are lit and SLAY Mode begins.</summary>
    public static event Action OnSlayModeStarted;

    /// <summary>Fires when SLAY Mode expires and the chain resets to zero.</summary>
    public static event Action OnSlayModeEnded;

    // ── Private ───────────────────────────────────────────────────────────────

    private float _decayTimer;
    private bool _decayActive;
    private Coroutine _slayModeCoroutine;

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
        if (IsSlayModeActive || !_decayActive || CurrentCount <= 0) return;

        _decayTimer -= Time.deltaTime;
        if (_decayTimer > 0f) return;

        // Step back one letter and restart the timer.
        CurrentCount--;
        OnSlayCountChanged?.Invoke(CurrentCount);
        Debug.Log($"[SlayService] Chain decayed → {CurrentCount}/4");

        if (CurrentCount > 0)
            _decayTimer = decayWindow;
        else
            _decayActive = false;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Called by an enemy death. Advances the kill-chain by one and resets the decay timer.
    /// Has no effect while SLAY Mode is active.
    /// </summary>
    public void RegisterKill()
    {
        if (IsSlayModeActive) return;

        CurrentCount = Mathf.Min(4, CurrentCount + 1);
        _decayTimer  = decayWindow;
        _decayActive = true;

        Debug.Log($"[SlayService] Kill registered → {CurrentCount}/4");
        OnSlayCountChanged?.Invoke(CurrentCount);

        if (CurrentCount >= 4)
            StartSlayMode();
    }

    /// <summary>
    /// Immediately resets the kill-chain to zero without triggering SLAY Mode.
    /// Call on ball drain. Has no effect while SLAY Mode is active.
    /// </summary>
    public void ResetChain()
    {
        if (IsSlayModeActive) return;

        CurrentCount = 0;
        _decayActive = false;
        OnSlayCountChanged?.Invoke(0);
        Debug.Log("[SlayService] Chain reset on drain.");
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void StartSlayMode()
    {
        IsSlayModeActive = true;
        _decayActive     = false;

        Debug.Log("[SlayService] *** SLAY MODE ACTIVATED ***");
        OnSlayModeStarted?.Invoke();

        if (_slayModeCoroutine != null) StopCoroutine(_slayModeCoroutine);
        _slayModeCoroutine = StartCoroutine(SlayModeRoutine());
    }

    private IEnumerator SlayModeRoutine()
    {
        yield return new WaitForSeconds(slayModeDuration);
        EndSlayMode();
    }

    private void EndSlayMode()
    {
        IsSlayModeActive   = false;
        CurrentCount       = 0;
        _slayModeCoroutine = null;

        Debug.Log("[SlayService] SLAY MODE ended — chain reset.");
        OnSlayModeEnded?.Invoke();
        OnSlayCountChanged?.Invoke(0);
    }
}
