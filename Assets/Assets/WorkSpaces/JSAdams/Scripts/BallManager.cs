
// ----- BallManager.cs START -----

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallManager : MonoBehaviour
{
    [Header("Prefab & Spawn")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Ball Limits")]
    [SerializeField] private int maxBallsOnTable = 3;

    [Header("Timing")]
    [Tooltip("How often we attempt to fill missing balls (while under cap).")]
    [SerializeField] private float refillCheckInterval = 0.5f;

    [Tooltip("Delay after a drain before a replacement spawns.")]
    [SerializeField] private float respawnDelayAfterDrain = 1.0f;

    [Tooltip("Delay between spawns when filling multiple missing balls.")]
    [SerializeField] private float staggerSpawnDelay = 0.35f;

    [Header("Spawn Polish")]
    [Tooltip("How long the ball hangs in the hole before physics starts.")]
    [SerializeField] private float spawnHangDuration = 0.20f;

    [Tooltip("Starting scale while hanging (1 = normal size).")]
    [Range(0.1f, 1f)]
    [SerializeField] private float spawnStartScale = 0.65f;

    [Tooltip("Extra little ease curve while scaling up.")]
    [SerializeField]
    private AnimationCurve spawnScaleEase =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private readonly HashSet<int> activeBallInstanceIds = new HashSet<int>();
    private Coroutine refillRoutine;

    private void OnEnable()
    {
        refillRoutine = StartCoroutine(RefillLoop());
    }

    private void OnDisable()
    {
        if (refillRoutine != null)
            StopCoroutine(refillRoutine);
    }

    private IEnumerator RefillLoop()
    {
        while (true)
        {
            int missing = maxBallsOnTable - activeBallInstanceIds.Count;

            if (missing > 0)
            {
                for (int i = 0; i < missing; i++)
                {
                    SpawnBall();
                    yield return new WaitForSeconds(staggerSpawnDelay);
                }
            }

            yield return new WaitForSeconds(refillCheckInterval);
        }
    }

    private void SpawnBall()
    {
        if (ballPrefab == null || spawnPoint == null)
        {
            Debug.LogError("[BallManager] Missing ballPrefab or spawnPoint reference.");
            return;
        }

        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);

        activeBallInstanceIds.Add(ball.GetInstanceID());

        // Polish: hang + scale up before releasing physics
        StartCoroutine(SpawnHangAndScale(ball));
    }

    private IEnumerator SpawnHangAndScale(GameObject ball)
    {
        if (ball == null) yield break;

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

        // Freeze physics during hang so it doesn't drift or bounce
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        Vector3 originalScale = ball.transform.localScale;
        Vector3 startScale = originalScale * Mathf.Max(0.01f, spawnStartScale);

        float t = 0f;
        while (t < spawnHangDuration)
        {
            if (ball == null) yield break;

            t += Time.deltaTime;
            float normalized = (spawnHangDuration <= 0f) ? 1f : Mathf.Clamp01(t / spawnHangDuration);

            float eased = spawnScaleEase != null ? spawnScaleEase.Evaluate(normalized) : normalized;

            ball.transform.position = spawnPoint.position; // stay locked to the hole
            ball.transform.localScale = Vector3.LerpUnclamped(startScale, originalScale, eased);

            yield return null;
        }

        if (ball == null) yield break;

        // Ensure final values
        ball.transform.position = spawnPoint.position;
        ball.transform.localScale = originalScale;

        // Release physics
        if (rb != null)
        {
            rb.simulated = true;
        }
    }

    /// <summary>
    /// Called by Drain when a ball enters the drain zone.
    /// Removes it and schedules a delayed refill (handled by the refill loop).
    /// </summary>
    public void BallDrained(GameObject ball)
    {
        if (ball == null) return;

        int id = ball.GetInstanceID();
        activeBallInstanceIds.Remove(id);

        Destroy(ball);

        StartCoroutine(DelayBeforeRefill());
    }

    private IEnumerator DelayBeforeRefill()
    {
        yield return new WaitForSeconds(respawnDelayAfterDrain);
        // refill loop will notice and spawn
    }

    public int GetActiveBallCount() => activeBallInstanceIds.Count;
}

// ----- BallManager.cs END -----