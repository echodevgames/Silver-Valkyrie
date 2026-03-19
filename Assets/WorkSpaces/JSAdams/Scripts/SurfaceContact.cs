using UnityEngine;

/// <summary>
/// Applies a SurfaceMaterialData's physics properties to every Collider2D on this GameObject
/// and fires screen shake and controller vibration feedback when the ball makes contact.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SurfaceContact : MonoBehaviour
{
    private const string BallTag = "Ball";

    [SerializeField] private SurfaceMaterialData surfaceMaterial;

    /// <summary>Exposes the active surface for query by other systems (scoring, audio, etc.).</summary>
    public SurfaceMaterialData SurfaceMaterial => surfaceMaterial;

    private void Awake()
    {
        ApplyPhysicsMaterial();
    }

    /// <summary>Pushes the assigned physics material onto all Collider2D components on this GameObject.</summary>
    public void ApplyPhysicsMaterial()
    {
        if (surfaceMaterial == null || surfaceMaterial.physicsMaterial == null)
        {
            Debug.LogWarning($"[SurfaceContact] No SurfaceMaterialData or PhysicsMaterial2D assigned on '{name}'.", this);
            return;
        }

        foreach (Collider2D col in GetComponents<Collider2D>())
            col.sharedMaterial = surfaceMaterial.physicsMaterial;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (surfaceMaterial == null) return;
        if (!col.gameObject.CompareTag(BallTag)) return;

        Vector3 contactPoint = col.GetContact(0).point;

        if (surfaceMaterial.shakeIntensity > 0f)
            ScreenShakeService.Instance?.Shake(surfaceMaterial.shakeIntensity);

        if (surfaceMaterial.vibrationIntensity > 0f)
            VibrationService.Instance?.Vibrate(surfaceMaterial.vibrationIntensity, surfaceMaterial.vibrationDuration);

        if (surfaceMaterial.bounceBoostScale > 0f && col.rigidbody != null)
        {
            Vector2 normal    = col.GetContact(0).normal;
            float   boostForce = surfaceMaterial.shakeIntensity * surfaceMaterial.bounceBoostScale;
            col.rigidbody.AddForce(normal * boostForce, ForceMode2D.Impulse);
        }

        AudioService.Instance?.Play(surfaceMaterial, contactPoint);

        if (surfaceMaterial.scoreValue != 0)
            ScoreService.Instance?.Add(surfaceMaterial.scoreValue);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyPhysicsMaterial();
    }
#endif
}
