using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Singleton service that drives gamepad rumble motors on hit events.
/// Uses low-frequency motor for the body thud and high-frequency motor for the surface snap.
/// Add this component to any persistent scene GameObject.
/// </summary>
public class VibrationService : MonoBehaviour
{
    public static VibrationService Instance { get; private set; }

    private Coroutine activeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            StopMotors();
            Instance = null;
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // Always stop motors when the app loses focus to avoid a stuck rumble.
        if (!hasFocus) StopMotors();
    }

    /// <summary>
    /// Vibrates the current gamepad at the given intensity for the specified duration.
    /// Concurrent calls interrupt the previous vibration.
    /// </summary>
    public void Vibrate(float intensity, float duration)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(VibrateRoutine(intensity, duration));
    }

    private IEnumerator VibrateRoutine(float intensity, float duration)
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) yield break;

        // Low motor = low-frequency body thud; high motor = high-frequency surface snap.
        gamepad.SetMotorSpeeds(intensity * 0.5f, intensity);

        yield return new WaitForSeconds(duration);

        gamepad.ResetHaptics();
        activeRoutine = null;
    }

    private void StopMotors()
    {
        Gamepad.current?.ResetHaptics();
    }
}
