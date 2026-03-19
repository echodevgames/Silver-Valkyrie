using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Standalone plunger controller. Hold the launch button to pull the ram back,
/// release to fire the ball up the lane.
/// </summary>
public class Plunger : MonoBehaviour
{
    private enum PlungerState { Idle, Pulling, Slamming, Settling }

    [Header("References")]
    [SerializeField] private Rigidbody2D ramRb;
    [SerializeField] private Transform housingTransform;

    [Header("Pull")]
    [Tooltip("How far the ram travels down when fully charged.")]
    [SerializeField] private float maxPullDistance = 1.5f;
    [Tooltip("Time in seconds to reach full pull from a fresh press.")]
    [SerializeField] private float pullDuration = 0.7f;

    [Header("Launch")]
    [Tooltip("Upward velocity applied to the ram on release.")]
    [SerializeField] private float launchSpeed = 35f;

    [Header("Settle")]
    [Tooltip("Peak overshoot distance after the slam.")]
    [SerializeField] private float overshootAmount = 0.15f;
    [Tooltip("Higher value stops oscillation faster.")]
    [SerializeField] private float settleDamping = 5f;
    [Tooltip("Higher value tightens the oscillation frequency.")]
    [SerializeField] private float settleFrequency = 14f;

    [Header("Housing Shake")]
    [SerializeField] private float shakeStrength = 0.06f;
    [SerializeField] private float shakeDuration = 0.12f;
    [Tooltip("Max screen shake intensity at full charge. Scales linearly with pull distance.")]
    [SerializeField] private float launchScreenShakeIntensity = 0.35f;

    [Header("Audio")]
    [Tooltip("Sound played when the plunger fires. Volume scales with charge ratio.")]
    [SerializeField] private AudioClip launchSound;
    [Range(0f, 1f)]
    [SerializeField] private float launchVolume = 0.9f;

    private PlungerState state = PlungerState.Idle;
    private Vector2 restPosition;
    private float pullTimer;
    private float settleTimer;

    private void Awake()
    {
        restPosition = ramRb.position;
        // Keep the ram Kinematic at rest so the ball's weight cannot push it down.
        // We only switch to Dynamic for the brief slam impulse.
        ramRb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnEnable()
    {
        var svInput = InputService.Instance?.Input;
        if (svInput == null)
        {
            Debug.LogError("[Plunger] InputService not ready — no input response.");
            return;
        }

        svInput.Gameplay.Launch.performed += OnPullStarted;
        svInput.Gameplay.Launch.canceled  += OnPullReleased;
    }

    private void OnDisable()
    {
        var svInput = InputService.Instance?.Input;
        if (svInput == null) return;

        svInput.Gameplay.Launch.performed -= OnPullStarted;
        svInput.Gameplay.Launch.canceled  -= OnPullReleased;
    }

    private void OnPullStarted(InputAction.CallbackContext ctx)
    {
        if (state != PlungerState.Idle) return;
        state = PlungerState.Pulling;
        pullTimer = 0f;
    }

    private void OnPullReleased(InputAction.CallbackContext ctx)
    {
        if (state != PlungerState.Pulling) return;

        float chargeRatio = Mathf.Clamp01(pullTimer / pullDuration);

        state = PlungerState.Slamming;

        // Switch to Dynamic so the ram delivers a real physics impulse to the ball.
        ramRb.bodyType = RigidbodyType2D.Dynamic;
        ramRb.linearVelocity = Vector2.up * launchSpeed;

        // Screen shake scales with how far the plunger was pulled.
        ScreenShakeService.Instance?.Shake(launchScreenShakeIntensity * chargeRatio);

        // Volume also scales with charge so a light tap sounds softer than a full pull.
        AudioService.Instance?.PlayOneShot(launchSound, transform.position, launchVolume * chargeRatio);

        StartCoroutine(ShakeHousing());
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case PlungerState.Pulling:  HandlePull();   break;
            case PlungerState.Slamming: HandleSlam();   break;
            case PlungerState.Settling: HandleSettle(); break;
        }
    }

    private void HandlePull()
    {
        pullTimer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(pullTimer / pullDuration);
        float pullOffset = EaseOut(t) * maxPullDistance;
        SetRamYOffset(-pullOffset); // negative = move ram downward
    }

    private void HandleSlam()
    {
        // Wait until the ram has passed its rest height on the way up.
        if (ramRb.position.y < restPosition.y) return;

        // Return to Kinematic before settling so nothing can push it around.
        ramRb.bodyType = RigidbodyType2D.Kinematic;
        ramRb.linearVelocity = Vector2.zero;
        settleTimer = 0f;
        state = PlungerState.Settling;
    }

    private void HandleSettle()
    {
        settleTimer += Time.fixedDeltaTime;

        float oscillation =
            overshootAmount *
            Mathf.Exp(-settleDamping * settleTimer) *
            Mathf.Cos(settleFrequency * settleTimer);

        SetRamYOffset(oscillation);

        if (settleTimer > 0.3f && Mathf.Abs(oscillation) < 0.005f)
        {
            SetRamYOffset(0f);
            state = PlungerState.Idle;
        }
    }

    /// <summary>Moves the ram to its rest position offset vertically by yOffset.</summary>
    private void SetRamYOffset(float yOffset)
    {
        ramRb.MovePosition(new Vector2(restPosition.x, restPosition.y + yOffset));
    }

    private IEnumerator ShakeHousing()
    {
        if (housingTransform == null) yield break;

        Vector3 origin = housingTransform.localPosition;
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            housingTransform.localPosition = origin + (Vector3)(Random.insideUnitCircle * shakeStrength);
            yield return null;
        }

        housingTransform.localPosition = origin;
    }

    /// <summary>Quadratic ease-out curve. t in 0..1, returns 0..1.</summary>
    private static float EaseOut(float t) => 1f - Mathf.Pow(1f - t, 2f);
}