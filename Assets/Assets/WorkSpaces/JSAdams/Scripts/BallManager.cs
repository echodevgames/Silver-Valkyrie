// ----- BallManager.cs START -----
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallManager : MonoBehaviour
{
    [Header("Prefab & Spawn")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Gate")]
    [SerializeField] private TroughGate troughGate;

    [Header("Ball Limits")]
    [SerializeField] private int maxBallsOnTable = 3;

    [Header("Timing")]
    [SerializeField] private float refillCheckInterval = 0.5f;
    [SerializeField] private float staggerSpawnDelay = 0.35f;
    [SerializeField] private float gameStartSpawnDelay = 0.5f;

    [Header("Spawn Polish")]
    [Tooltip("How long the ball hangs in the hole before physics starts.")]
    [SerializeField] private float spawnHangDuration = 0.20f;

    [Tooltip("Starting scale while hanging (1 = normal size).")]
    [Range(0.1f, 1f)]
    [SerializeField] private float spawnStartScale = 0.65f;

    [Tooltip("Extra little ease curve while scaling up.")]
    [SerializeField]
    private AnimationCurve spawnScaleEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private readonly List<GameObject> activeBalls = new();
    private GameObject primaryBall;
    private Coroutine refillRoutine;
    private bool hasStarted = false;

    public void StartSpawning()
    {
        if (hasStarted) return;
        hasStarted = true;

        StartCoroutine(StartAfterDelay());
    }

    private void OnDisable()
    {
        if (refillRoutine != null)
            StopCoroutine(refillRoutine);
    }

    private IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(gameStartSpawnDelay);
        refillRoutine = StartCoroutine(RefillLoop());
    }

    private IEnumerator RefillLoop()
    {
        while (true)
        {
            int missing = maxBallsOnTable - activeBalls.Count;

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
            Debug.LogError("[BallManager] Missing ballPrefab or spawnPoint.");
            return;
        }

        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);

        BallState state = ball.GetComponent<BallState>();
        if (state != null)
            state.SetState(BallState.State.InTrough);

        activeBalls.Add(ball);

        // Restore hang effect
        StartCoroutine(SpawnHangAndScale(ball));

        if (primaryBall == null)
        {
            SetPrimaryBall(ball);
        }
    }

    private void SetPrimaryBall(GameObject ball)
    {
        primaryBall = ball;

        if (ZoneCameraManager.Instance != null)
            ZoneCameraManager.Instance.SetPrimaryBall(ball.transform);

        BallState state = ball.GetComponent<BallState>();
        if (state != null)
        {
            // Reset primary lifecycle to trough until it exits via ReleaseTrigger
            state.SetState(BallState.State.InTrough);

            // Avoid double-subscribe if something calls SetPrimaryBall twice
            state.OnStateChanged -= HandlePrimaryBallStateChanged;
            state.OnStateChanged += HandlePrimaryBallStateChanged;

            // Apply immediately
            HandlePrimaryBallStateChanged(state.CurrentState);
        }
        else
        {
            // If there's no BallState, safest is to keep the gate closed
            troughGate.ForceClose();
        }
    }

    private void HandlePrimaryBallStateChanged(BallState.State newState)
    {
        switch (newState)
        {
            case BallState.State.InTrough:
                troughGate.ForceOpen();
                break;

            case BallState.State.InPlunger:
            case BallState.State.InPlayfield:
                troughGate.ForceClose();
                break;

            case BallState.State.Drained:
                troughGate.ForceOpen();
                break;
        }
    }

    public void BallDrained(GameObject ball)
    {
        if (ball == null) return;

        bool wasPrimary = (ball == primaryBall);

        BallState oldState = ball.GetComponent<BallState>();
        if (oldState != null)
        {
            // optional but consistent
            oldState.SetState(BallState.State.Drained);
            oldState.OnStateChanged -= HandlePrimaryBallStateChanged;
        }

        activeBalls.Remove(ball);
        Destroy(ball);

        if (wasPrimary)
        {
            if (activeBalls.Count > 0)
            {
                SetPrimaryBall(activeBalls[0]);
            }
            else
            {
                primaryBall = null;
                troughGate.ForceClose();
            }
        }
    }

    private IEnumerator SpawnHangAndScale(GameObject ball)
    {
        if (ball == null) yield break;

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

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
            float eased = (spawnScaleEase != null) ? spawnScaleEase.Evaluate(normalized) : normalized;

            ball.transform.position = spawnPoint.position;
            ball.transform.localScale = Vector3.LerpUnclamped(startScale, originalScale, eased);

            yield return null;
        }

        if (ball == null) yield break;

        ball.transform.position = spawnPoint.position;
        ball.transform.localScale = originalScale;

        if (rb != null)
            rb.simulated = true;
    }

    public void DebugDrainPrimaryBall()
    {
        if (primaryBall == null)
        {
            Debug.Log("No primary ball to drain.");
            return;
        }

        BallDrained(primaryBall);
    }
}
// ----- BallManager.cs END -----