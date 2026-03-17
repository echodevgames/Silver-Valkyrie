using System;
using UnityEngine;

/// <summary>
/// Trigger zone placed below the flippers. When the ball enters it,
/// feedback fires and the ball is destroyed. Subscribe to OnBallDrained
/// to hook in a future life/respawn system.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BallDrain : MonoBehaviour
{
    private const string BallTag = "Ball";

    [Header("Feedback")]
    [Tooltip("Screen shake intensity when the ball drains.")]
    [Range(0f, 1f)]
    [SerializeField] private float shakeIntensity = 0.8f;

    [Tooltip("Gamepad rumble intensity on drain.")]
    [Range(0f, 1f)]
    [SerializeField] private float vibrationIntensity = 0.9f;

    [Tooltip("How long the rumble lasts in seconds.")]
    [SerializeField] private float vibrationDuration = 0.4f;

    [Header("Audio")]
    [Tooltip("Sound played when the ball drains. Leave empty until you have a recording.")]
    [SerializeField] private AudioClip drainSound;

    [Range(0f, 1f)]
    [SerializeField] private float drainVolume = 1f;

    /// <summary>Fires when the ball enters the drain zone. Parameter is the ball GameObject.</summary>
    public static event Action<GameObject> OnBallDrained;

    private void Awake()
    {
        // Ensure the collider on this object is always a trigger.
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(BallTag)) return;

        Debug.Log("[BallDrain] Ball drained.");

        ScreenShakeService.Instance?.Shake(shakeIntensity);
        VibrationService.Instance?.Vibrate(vibrationIntensity, vibrationDuration);
        AudioService.Instance?.PlayOneShot(drainSound, transform.position, drainVolume);

        OnBallDrained?.Invoke(other.gameObject);

        Destroy(other.gameObject);
    }
}
