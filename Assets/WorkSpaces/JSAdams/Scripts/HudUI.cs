using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives the always-visible gameplay HUD side panel. Subscribes to event streams from
/// <see cref="ScoreService"/>, <see cref="BallLifeService"/>, <see cref="BallRegistry"/>,
/// <see cref="LivesService"/>, <see cref="SlayService"/>, and <see cref="ComboService"/>.
///
/// SLAY pips (4 Images, S→L→A→Y order) light gold as the kill-chain builds and flash
/// white during SLAY Mode. Continue pips (up to 3 Images) reflect remaining continues.
/// Wire all serialized fields in the Inspector.
/// </summary>
public class HudUI : MonoBehaviour
{
    // ── Pip colors (from UI Style Guide) ──────────────────────────────────────

    private static readonly Color SlayLit    = new Color(0.910f, 0.784f, 0.251f, 1.00f); // text-heading gold
    private static readonly Color SlayDim    = new Color(0.533f, 0.565f, 0.596f, 0.35f); // text-muted dim
    private static readonly Color SlayFlash  = new Color(1.000f, 1.000f, 1.000f, 1.00f); // white flash
    private static readonly Color ContActive = new Color(0.604f, 0.667f, 0.722f, 1.00f); // border-silver
    private static readonly Color ContSpent  = new Color(0.039f, 0.039f, 0.110f, 0.80f); // near-black

    // ── Inspector fields ──────────────────────────────────────────────────────

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreValue;

    [Header("High Score")]
    [SerializeField] private TextMeshProUGUI hiScoreValue;

    [Header("SLAY Chain")]
    [Tooltip("Exactly four pip Images in S → L → A → Y order.")]
    [SerializeField] private Image[] slayPips = new Image[4];

    [Header("Combo")]
    [SerializeField] private TextMeshProUGUI comboValue;

    [Header("Balls Available")]
    [SerializeField] private TextMeshProUGUI ballsValue;

    [Header("Balls In Play")]
    [SerializeField] private TextMeshProUGUI inPlayValue;

    [Header("Continues")]
    [Tooltip("One pip Image per continue slot. Length must match LivesService.startingLives.")]
    [SerializeField] private Image[] continuePips = new Image[3];

    // ── Private ───────────────────────────────────────────────────────────────

    private Coroutine _slayFlashRoutine;
    private bool      _scoreSubscribed;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        // ScoreService uses an instance event — subscribe via helper to handle
        // the case where Instance might not yet exist this early in the frame.
        SubscribeScoreService();

        BallLifeService.OnBallsChanged  += OnBallsChanged;
        BallRegistry.OnBallCountChanged += OnBallCountChanged;
        LivesService.OnLivesChanged     += OnContinuesChanged;

        SlayService.OnSlayCountChanged += OnSlayCountChanged;
        SlayService.OnSlayModeStarted  += OnSlayModeStarted;
        SlayService.OnSlayModeEnded    += OnSlayModeEnded;

        ComboService.OnComboChanged += OnComboChanged;
    }

    private void OnDisable()
    {
        if (ScoreService.Instance != null)
            ScoreService.Instance.OnScoreChanged -= OnScoreChanged;
        _scoreSubscribed = false;

        BallLifeService.OnBallsChanged  -= OnBallsChanged;
        BallRegistry.OnBallCountChanged -= OnBallCountChanged;
        LivesService.OnLivesChanged     -= OnContinuesChanged;

        SlayService.OnSlayCountChanged -= OnSlayCountChanged;
        SlayService.OnSlayModeStarted  -= OnSlayModeStarted;
        SlayService.OnSlayModeEnded    -= OnSlayModeEnded;

        ComboService.OnComboChanged -= OnComboChanged;

        StopSlayFlash();
    }

    private void Start()
    {
        // Instance events require Instance to exist — guaranteed by Start() since
        // all Awake() calls across the scene complete before any Start() runs.
        SubscribeScoreService();

        // Push initial values so the HUD is never blank on first frame.
        if (scoreValue  != null) scoreValue.text  = "0";
        if (ballsValue  != null) ballsValue.text  = (BallLifeService.Instance?.BallsRemaining ?? 0).ToString();
        if (inPlayValue != null) inPlayValue.text = (BallRegistry.Instance?.BallCount ?? 0).ToString();
        if (comboValue  != null) comboValue.text  = "×0";

        int topScore = (HighScoreService.Instance != null && HighScoreService.Instance.Entries.Count > 0)
            ? HighScoreService.Instance.Entries[0].score
            : 0;
        if (hiScoreValue != null) hiScoreValue.text = topScore.ToString("N0");

        RefreshSlayPips(SlayService.Instance?.CurrentCount ?? 0);
        RefreshContinuePips(LivesService.Instance?.LivesRemaining ?? 0);
    }

    /// <summary>
    /// Subscribes to ScoreService.OnScoreChanged exactly once.
    /// Safe to call from both OnEnable and Start — guards against double-subscription
    /// and silently skips if Instance is not yet initialised.
    /// </summary>
    private void SubscribeScoreService()
    {
        if (_scoreSubscribed || ScoreService.Instance == null) return;
        ScoreService.Instance.OnScoreChanged += OnScoreChanged;
        _scoreSubscribed = true;
    }

    // ── Score ─────────────────────────────────────────────────────────────────

    private void OnScoreChanged(int _, int total)
    {
        if (scoreValue != null) scoreValue.text = total.ToString("N0");
    }

    // ── Balls ─────────────────────────────────────────────────────────────────

    private void OnBallsChanged(int remaining)
    {
        if (ballsValue != null) ballsValue.text = remaining.ToString();
    }

    private void OnBallCountChanged(int inPlay)
    {
        if (inPlayValue != null) inPlayValue.text = inPlay.ToString();
    }

    // ── Continues ─────────────────────────────────────────────────────────────

    private void OnContinuesChanged(int count) => RefreshContinuePips(count);

    /// <summary>Colors pip images silver when active, near-black when spent.</summary>
    private void RefreshContinuePips(int activeCount)
    {
        for (int i = 0; i < continuePips.Length; i++)
        {
            if (continuePips[i] == null) continue;
            continuePips[i].color = i < activeCount ? ContActive : ContSpent;
        }
    }

    // ── SLAY chain ────────────────────────────────────────────────────────────

    private void OnSlayCountChanged(int count)
    {
        if (SlayService.Instance == null || !SlayService.Instance.IsSlayModeActive)
            RefreshSlayPips(count);
    }

    private void OnSlayModeStarted()
    {
        StopSlayFlash();
        _slayFlashRoutine = StartCoroutine(SlayModeFlash());
    }

    private void OnSlayModeEnded()
    {
        StopSlayFlash();
        RefreshSlayPips(0);
    }

    /// <summary>Lights each pip gold up to litCount; dims the rest.</summary>
    private void RefreshSlayPips(int litCount)
    {
        for (int i = 0; i < slayPips.Length; i++)
        {
            if (slayPips[i] == null) continue;
            slayPips[i].color = i < litCount ? SlayLit : SlayDim;
        }
    }

    private IEnumerator SlayModeFlash()
    {
        bool toggle = false;
        while (true)
        {
            Color c = toggle ? SlayFlash : SlayLit;
            foreach (Image pip in slayPips)
                if (pip != null) pip.color = c;
            toggle = !toggle;
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void StopSlayFlash()
    {
        if (_slayFlashRoutine == null) return;
        StopCoroutine(_slayFlashRoutine);
        _slayFlashRoutine = null;
    }

    // ── Combo ─────────────────────────────────────────────────────────────────

    private void OnComboChanged(int combo)
    {
        if (comboValue != null)
            comboValue.text = combo > 1 ? $"×{combo}" : "×0";
    }
}
