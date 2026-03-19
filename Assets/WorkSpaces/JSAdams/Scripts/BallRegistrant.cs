using UnityEngine;

/// <summary>
/// Added to the Ball prefab. Self-registers with BallRegistry on spawn
/// and deregisters when destroyed — no manual wiring required.
/// </summary>
public class BallRegistrant : MonoBehaviour
{
    private static int _spawnCounter;

    /// <summary>
    /// Monotonically increasing index assigned at spawn time.
    /// Lower value = older ball. Used as a tiebreaker when two balls share the same Y.
    /// </summary>
    public int SpawnIndex { get; private set; }

    // Start() runs after all Awake()s, guaranteeing BallRegistry.Instance exists.
    private void Start()
    {
        SpawnIndex = _spawnCounter++;
        BallRegistry.Instance?.Register(this);
    }

    private void OnDestroy()
    {
        BallRegistry.Instance?.Deregister(this);
    }
}
