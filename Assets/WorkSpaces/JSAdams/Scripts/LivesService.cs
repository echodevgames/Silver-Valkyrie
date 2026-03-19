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

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Resets lives back to the starting amount for a new session.</summary>
    public void ResetLives()
    {
        LivesRemaining = startingLives;
        OnLivesChanged?.Invoke(LivesRemaining);
    }

    /// <summary>
    /// Spends one continue to resume play. Resets the ball pool and returns true if successful.
    /// Returns false (and fires <see cref="OnTrueGameOver"/>) if no continues remain.
    /// </summary>
    public bool UseContinue()
    {
        if (LivesRemaining <= 0)
        {
            Debug.Log("[LivesService] No continues remaining — true game over.");
            OnTrueGameOver?.Invoke();
            return false;
        }

        LivesRemaining = Mathf.Max(0, LivesRemaining - 1);
        Debug.Log($"[LivesService] Continue used. Remaining: {LivesRemaining}");
        OnLivesChanged?.Invoke(LivesRemaining);
        BallLifeService.Instance?.ResetBalls();
        return true;
    }
}
