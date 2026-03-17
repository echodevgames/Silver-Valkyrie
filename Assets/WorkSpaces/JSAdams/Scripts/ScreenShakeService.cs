using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton service that applies a decaying random offset to the camera via CameraFollow.ShakeOffset.
/// Add this component to any persistent scene GameObject.
/// </summary>
public class ScreenShakeService : MonoBehaviour
{
    public static ScreenShakeService Instance { get; private set; }

    [Tooltip("World-units magnitude at intensity 1.")]
    [SerializeField] private float maxMagnitude = 0.25f;

    [Tooltip("How quickly the shake decays. Higher = shorter shake.")]
    [SerializeField] private float dampingSpeed = 10f;

    private CameraFollow cameraFollow;
    private float currentIntensity;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        cameraFollow = Camera.main?.GetComponent<CameraFollow>();

        if (cameraFollow == null)
            Debug.LogWarning("[ScreenShakeService] No CameraFollow found on Main Camera — shake will have no effect.", this);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Triggers a screen shake scaled by intensity (0–1).</summary>
    public void Shake(float intensity)
    {
        if (cameraFollow == null) return;

        // Accumulate so rapid hits stack without restarting.
        currentIntensity = Mathf.Max(currentIntensity, intensity * maxMagnitude);

        if (shakeRoutine != null) return;
        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        while (currentIntensity > 0.001f)
        {
            Vector2 offset = Random.insideUnitCircle * currentIntensity;
            cameraFollow.ShakeOffset = new Vector3(offset.x, offset.y, 0f);

            currentIntensity = Mathf.Lerp(currentIntensity, 0f, dampingSpeed * Time.deltaTime);
            yield return null;
        }

        cameraFollow.ShakeOffset = Vector3.zero;
        currentIntensity = 0f;
        shakeRoutine = null;
    }
}
