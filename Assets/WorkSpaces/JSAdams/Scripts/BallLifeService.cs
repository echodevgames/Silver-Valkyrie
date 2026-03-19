using System;
using UnityEngine;

/// <summary>
/// Tracks how many balls the player has remaining in the current session.
/// Subscribes to BallDrain.OnBallDrained — no other script needs to call this directly.
/// Fires OnBallLost when a ball is lost but more remain, or OnGameOver when the pool hits zero.
/// Call AddBalls() to grant extra balls from power-ups, multiball, etc.
/// </summary>
public class BallLifeService : MonoBehaviour
{
    public static BallLifeService Instance { get; private set; }

    [Tooltip("Number of balls the player starts each session with.")]
    [SerializeField] private int startingBalls = 3;

    public int BallsRemaining { get; private set; }

    /// <summary>Fired when a ball drains and at least one ball remains. Parameter = balls left.</summary>
    public static event Action<int> OnBallLost;

    /// <summary>Fired whenever the ball count changes (drain or bonus). Parameter = new total.</summary>
    public static event Action<int> OnBallsChanged;

    /// <summary>Fired when the last ball drains.</summary>
    public static event Action OnGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        BallsRemaining = startingBalls;
        BallDrain.OnBallDrained += HandleBallDrained;
    }

    private void OnDisable()
    {
        BallDrain.OnBallDrained -= HandleBallDrained;
    }

    private void HandleBallDrained(GameObject _)
    {
        BallsRemaining = Mathf.Max(0, BallsRemaining - 1);
        Debug.Log($"[BallLifeService] Ball drained. Remaining: {BallsRemaining}");

        OnBallsChanged?.Invoke(BallsRemaining);

        if (BallsRemaining <= 0)
            OnGameOver?.Invoke();
        else
            OnBallLost?.Invoke(BallsRemaining);
    }

    /// <summary>Adds balls to the current pool. Safe to call from power-ups, multiball grants, cheats, etc.</summary>
    public void AddBalls(int count)
    {
        if (count <= 0) return;

        BallsRemaining += count;
        Debug.Log($"[BallLifeService] +{count} ball(s). Now: {BallsRemaining}");
        OnBallsChanged?.Invoke(BallsRemaining);
    }

    /// <summary>
    /// Manually registers a drain without requiring the ball to pass through the drain trigger.
    /// Use for cheat kills, scripted events, or multiball edge cases.
    /// </summary>
    public void SimulateDrain()
    {
        HandleBallDrained(null);
    }

    /// <summary>Resets the ball count back to the starting value.</summary>
    public void ResetBalls()
    {
        BallsRemaining = startingBalls;
        OnBallsChanged?.Invoke(BallsRemaining);
    }
}
