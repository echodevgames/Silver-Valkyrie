using System;
using UnityEngine;

/// <summary>
/// Tracks the player's remaining lives (continues). A life is consumed each time
/// <see cref="BallLifeService.OnGameOver"/> fires (all balls drained). When a life
/// remains, the ball bank is automatically reset so play continues. When the last
/// life is spent, <see cref="OnTrueGameOver"/> fires for the game-state layer to handle.
/// </summary>
public class LivesService : MonoBehaviour
{
    public static LivesService Instance { get; private set; }

    [Tooltip("How many lives the player starts with each session.")]
    [SerializeField] private int startingLives = 3;

    /// <summary>Lives remaining this session.</summary>
    public int LivesRemaining { get; private set; }

    /// <summary>Fired when the life count changes. Parameter = new remaining count.</summary>
    public static event Action<int> OnLivesChanged;

    /// <summary>Fired when the player has used all lives and game is truly over.</summary>
    public static event Action OnTrueGameOver;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        LivesRemaining = startingLives;
    }

    private void OnEnable()  => BallLifeService.OnGameOver += HandleGameOver;
    private void OnDisable() => BallLifeService.OnGameOver -= HandleGameOver;

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Resets lives back to the starting amount for a new session.</summary>
    public void ResetLives()
    {
        LivesRemaining = startingLives;
        OnLivesChanged?.Invoke(LivesRemaining);
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private void HandleGameOver()
    {
        LivesRemaining = Mathf.Max(0, LivesRemaining - 1);
        Debug.Log($"[LivesService] Life consumed. Remaining: {LivesRemaining}");
        OnLivesChanged?.Invoke(LivesRemaining);

        if (LivesRemaining > 0)
            BallLifeService.Instance?.ResetBalls();
        else
            OnTrueGameOver?.Invoke();
    }
}
