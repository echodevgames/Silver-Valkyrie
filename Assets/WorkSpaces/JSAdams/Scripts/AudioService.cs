using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton service that plays surface hit sounds.
/// If a SurfaceMaterialData has no AudioClip assigned, a procedural placeholder
/// tone is generated and played instead — distinct per SurfaceType.
/// </summary>
public class AudioService : MonoBehaviour
{
    public static AudioService Instance { get; private set; }

    private const int SampleRate = 44100;

    // Procedural tone parameters per surface type: (frequency Hz, decay rate, duration s)
    private static readonly Dictionary<SurfaceMaterialData.SurfaceType, (float freq, float decay, float duration)> ToneParams =
        new()
        {
            { SurfaceMaterialData.SurfaceType.Rubber, (180f,   8f, 0.20f) }, // bassy thwack
            { SurfaceMaterialData.SurfaceType.Metal,  (1200f, 20f, 0.15f) }, // sharp ping
            { SurfaceMaterialData.SurfaceType.Stone,  (90f,   6f, 0.25f)  }, // deep thump
            { SurfaceMaterialData.SurfaceType.Wood,   (320f,  10f, 0.18f) }, // mid knock
            { SurfaceMaterialData.SurfaceType.Ice,    (700f,  25f, 0.12f) }, // crisp click
            { SurfaceMaterialData.SurfaceType.Custom, (440f,  12f, 0.15f) }, // neutral tone
        };

    private Dictionary<SurfaceMaterialData.SurfaceType, AudioClip> proceduralClips;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        BuildProceduralClips();

        // 2D audio source — spatialBlend 0 means distance has no effect on volume.
        audioSource             = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake  = false;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Plays the hit sound for the given surface.</summary>
    public void Play(SurfaceMaterialData surface, Vector3 worldPosition)
    {
        if (surface == null) return;

        AudioClip clip = surface.hitSound != null
            ? surface.hitSound
            : GetProceduralClip(surface.surfaceType);

        if (clip != null)
            audioSource.PlayOneShot(clip, surface.hitVolume);
    }

    /// <summary>Plays a one-shot clip — for non-surface events like plunger launch.</summary>
    public void PlayOneShot(AudioClip clip, Vector3 worldPosition, float volume = 1f)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip, volume);
    }

    private AudioClip GetProceduralClip(SurfaceMaterialData.SurfaceType type)
    {
        proceduralClips.TryGetValue(type, out AudioClip clip);
        return clip;
    }

    private void BuildProceduralClips()
    {
        proceduralClips = new Dictionary<SurfaceMaterialData.SurfaceType, AudioClip>();

        foreach (var kvp in ToneParams)
            proceduralClips[kvp.Key] = BuildTone(kvp.Key.ToString(), kvp.Value.freq, kvp.Value.decay, kvp.Value.duration);
    }

    /// <summary>Generates a sine-wave tone with an exponential decay envelope.</summary>
    private static AudioClip BuildTone(string clipName, float frequency, float decayRate, float duration)
    {
        int sampleCount = Mathf.RoundToInt(SampleRate * duration);
        float[] data = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t        = i / (float)SampleRate;
            float envelope = Mathf.Exp(-decayRate * t);
            data[i]        = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope;
        }

        AudioClip clip = AudioClip.Create($"Procedural_{clipName}", sampleCount, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
