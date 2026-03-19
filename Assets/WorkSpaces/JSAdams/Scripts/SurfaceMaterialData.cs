using UnityEngine;

/// <summary>
/// Data container defining the physical and sensory properties of a game surface.
/// Create instances via Assets > Create > Silver Valkyrie > Surface Material.
/// Assign to a SurfaceContact component to apply to any collider in the scene.
/// </summary>
[CreateAssetMenu(fileName = "Surface_New", menuName = "Silver Valkyrie/Surface Material")]
public class SurfaceMaterialData : ScriptableObject
{
    public enum SurfaceType { Rubber, Wood, Metal, Stone, Bumper, Target, Ice, Custom }

    [Header("Identity")]
    [Tooltip("Descriptive category used by future systems (audio, VFX, scoring).")]
    public SurfaceType surfaceType;

    [Header("Physics")]
    [Tooltip("Physics material applied to all Collider2Ds on the owning GameObject.")]
    public PhysicsMaterial2D physicsMaterial;

    [Header("Hit Feedback")]
    [Range(0f, 1f)]
    [Tooltip("How violently the camera shakes when the ball hits this surface.")]
    public float shakeIntensity;

    [Tooltip("Extra impulse force (world units) applied to the ball along the contact normal. Scaled by Shake Intensity — higher shake = stronger kick.")]
    public float bounceBoostScale;

    [Range(0f, 1f)]
    [Tooltip("Gamepad rumble strength on hit. Low motor = subtle thud, high motor = sharp snap.")]
    public float vibrationIntensity;

    [Tooltip("How long (seconds) the gamepad vibration lasts.")]
    public float vibrationDuration = 0.08f;

    [Header("Audio")]
    [Tooltip("Sound played on ball contact. Leave empty to use a procedural placeholder tone.")]
    public AudioClip hitSound;

    [Range(0f, 1f)]
    [Tooltip("Playback volume for the hit sound.")]
    public float hitVolume = 0.8f;

    [Header("Scoring")]
    [Tooltip("Points awarded to the player when the ball contacts this surface.")]
    public int scoreValue;
}

