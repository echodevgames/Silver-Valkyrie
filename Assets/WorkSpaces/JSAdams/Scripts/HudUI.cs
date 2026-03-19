using TMPro;
using UnityEngine;

/// <summary>
/// Drives the always-visible gameplay HUD panel. Subscribes to event streams from
/// <see cref="ScoreService"/>, <see cref="BallLifeService"/>, <see cref="BallRegistry"/>,
/// and <see cref="LivesService"/> and reflects state in the UI each time something changes.
/// Wire all serialized fields via Inspector.
/// </summary>
public class HudUI : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────────────

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreValue;

    [Header("Balls Available")]
    [SerializeField] private TextMeshProUGUI ballsValue;

    [Header("Balls In Play")]
    [SerializeField] private TextMeshProUGUI inPlayValue;

    [Header("Lives")]
    [SerializeField] private TextMeshProUGUI livesValue;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        if (ScoreService.Instance != null)
            ScoreService.Instance.OnScoreChanged += OnScoreChanged;

        BallLifeService.OnBallsChanged  += OnBallsChanged;
        BallRegistry.OnBallCountChanged += OnBallCountChanged;
        LivesService.OnLivesChanged     += OnLivesChanged;
    }

    private void OnDisable()
    {
        if (ScoreService.Instance != null)
            ScoreService.Instance.OnScoreChanged -= OnScoreChanged;

        BallLifeService.OnBallsChanged  -= OnBallsChanged;
        BallRegistry.OnBallCountChanged -= OnBallCountChanged;
        LivesService.OnLivesChanged     -= OnLivesChanged;
    }

    private void Start()
    {
        // Push initial values so the HUD is never blank on load.
        if (scoreValue  != null) scoreValue.text  = "0";
        if (ballsValue  != null) ballsValue.text  = (BallLifeService.Instance?.BallsRemaining ?? 0).ToString();
        if (inPlayValue != null) inPlayValue.text = (BallRegistry.Instance?.BallCount ?? 0).ToString();
        if (livesValue  != null) livesValue.text  = (LivesService.Instance?.LivesRemaining ?? 0).ToString();
    }

    // ── Event handlers ────────────────────────────────────────────────────────

    private void OnScoreChanged(int _, int total)
    {
        if (scoreValue != null) scoreValue.text = total.ToString("N0");
    }

    private void OnBallsChanged(int remaining)
    {
        if (ballsValue != null) ballsValue.text = remaining.ToString();
    }

    private void OnBallCountChanged(int inPlay)
    {
        if (inPlayValue != null) inPlayValue.text = inPlay.ToString();
    }

    private void OnLivesChanged(int lives)
    {
        if (livesValue != null) livesValue.text = lives.ToString();
    }
}
