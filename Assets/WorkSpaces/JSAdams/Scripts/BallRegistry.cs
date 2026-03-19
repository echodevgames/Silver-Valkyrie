using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton index of every active ball in the scene, ordered by spawn time (oldest = index 0).
///
/// "Primary ball" is defined as the ball with the lowest Y position — the one closest
/// to the drain and most at risk. Spawn order breaks ties.
///
/// Subscribe to OnPrimaryChanged to react when the focus ball changes.
/// CameraFollow and ZoneCameraManager both listen to this event.
/// </summary>
public class BallRegistry : MonoBehaviour
{
    public static BallRegistry Instance { get; private set; }

    private readonly List<BallRegistrant> _balls = new();
    private BallRegistrant _primary;

    /// <summary>
    /// Fired when the primary ball object changes — not every frame the ball moves.
    /// Parameter is the new primary's Transform, or null when no balls remain.
    /// </summary>
    public static event Action<Transform> OnPrimaryChanged;

    /// <summary>Fired whenever the active ball count changes. Parameter = new total.</summary>
    public static event Action<int> OnBallCountChanged;

    /// <summary>Transform of the current primary ball, or null if no balls are active.</summary>
    public Transform PrimaryBall => _primary != null ? _primary.transform : null;

    /// <summary>All currently active balls, oldest first.</summary>
    public IReadOnlyList<BallRegistrant> Balls => _balls;

    public int BallCount => _balls.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>Called by BallRegistrant.Start() when a ball enters the scene.</summary>
    public void Register(BallRegistrant ball)
    {
        if (!_balls.Contains(ball))
            _balls.Add(ball);

        OnBallCountChanged?.Invoke(_balls.Count);
        EvaluatePrimary();
    }

    /// <summary>Called by BallRegistrant.OnDestroy() when a ball is removed.</summary>
    public void Deregister(BallRegistrant ball)
    {
        _balls.Remove(ball);
        OnBallCountChanged?.Invoke(_balls.Count);
        EvaluatePrimary();
    }

    private void Update()
    {
        // Re-evaluate every frame so the camera switches to whichever ball
        // drops lowest — this is what makes multi-ball feel correct.
        if (_balls.Count > 1)
            EvaluatePrimary();
    }

    private void EvaluatePrimary()
    {
        if (_balls.Count == 0)
        {
            SetPrimary(null);
            return;
        }

        BallRegistrant candidate = null;

        foreach (BallRegistrant b in _balls)
        {
            if (b == null) continue; // guard against pending-destroy refs

            if (candidate == null)
            {
                candidate = b;
                continue;
            }

            float bY         = b.transform.position.y;
            float candidateY = candidate.transform.position.y;

            bool lowerY     = bY < candidateY;
            bool tiedY      = Mathf.Approximately(bY, candidateY);
            bool olderOnTie = tiedY && b.SpawnIndex < candidate.SpawnIndex;

            if (lowerY || olderOnTie)
                candidate = b;
        }

        SetPrimary(candidate);
    }

    private void SetPrimary(BallRegistrant ball)
    {
        if (_primary == ball) return;

        _primary = ball;
        OnPrimaryChanged?.Invoke(ball != null ? ball.transform : null);
        Debug.Log($"[BallRegistry] Primary ball changed → {(ball != null ? ball.name : "none")} | Total: {_balls.Count}");
    }
}
