using UnityEngine;

/// <summary>
/// Scales the TrailRenderer's width and duration with the ball's speed.
/// At low speed the trail is barely visible; at high speed it grows to dominate
/// the visual, naturally masking animation frame-stepping.
/// </summary>
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class BallTrail : MonoBehaviour
{
    [Header("Width (world units)")]
    [Tooltip("Trail width when the ball is nearly still.")]
    [SerializeField] private float minWidth = 0.04f;
    [Tooltip("Trail width at reference speed.")]
    [SerializeField] private float maxWidth = 0.45f;

    [Header("Duration (seconds)")]
    [Tooltip("Trail lifetime when nearly still.")]
    [SerializeField] private float minTime = 0.02f;
    [Tooltip("Trail lifetime at reference speed.")]
    [SerializeField] private float maxTime = 0.18f;

    [Header("Speed")]
    [Tooltip("Speed at which the trail reaches its maximum size. Match this to a typical fast launch speed.")]
    [SerializeField] private float referenceSpeed = 22f;

    [Tooltip("Minimum speed before the trail emits at all.")]
    [SerializeField] private float emitThreshold = 1f;

    private TrailRenderer trail;
    private Rigidbody2D   rb;

    private void Awake()
    {
        rb    = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
    }

    private void FixedUpdate()
    {
        float speed = rb.linearVelocity.magnitude;
        float t     = Mathf.Clamp01(speed / referenceSpeed);

        trail.emitting   = speed > emitThreshold;
        trail.time       = Mathf.Lerp(minTime, maxTime, t);
        trail.startWidth = Mathf.Lerp(minWidth, maxWidth, t);
        trail.endWidth   = 0f;
    }
}
