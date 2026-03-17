using System;
using UnityEngine;

/// <summary>
/// Singleton service that tracks the running score and broadcasts changes.
/// Subscribe to OnScoreChanged for UI updates. Points are logged to the console during development.
/// </summary>
public class ScoreService : MonoBehaviour
{
    public static ScoreService Instance { get; private set; }

    /// <summary>Fires when the score changes. Parameters: (pointsAdded, newTotal).</summary>
    public event Action<int, int> OnScoreChanged;

    /// <summary>Current accumulated score for this session.</summary>
    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Adds points to the score and notifies listeners.</summary>
    public void Add(int points)
    {
        if (points == 0) return;

        Score += points;
        Debug.Log($"[Score] +{points} → {Score}");
        OnScoreChanged?.Invoke(points, Score);
    }

    /// <summary>Resets the score back to zero.</summary>
    public void Reset()
    {
        Score = 0;
        Debug.Log("[Score] Reset → 0");
        OnScoreChanged?.Invoke(0, 0);
    }
}
