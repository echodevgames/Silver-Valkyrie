using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Singleton service that plays a looping background music track.
/// Assign a music clip in the Inspector — any .wav, .mp3, or .ogg imported into the project.
/// The track starts automatically on Awake if a clip is assigned.
/// </summary>
public class MusicService : MonoBehaviour
{
    public static MusicService Instance { get; private set; }

    [Header("Music")]
    [Tooltip("The background music track. Any loopable AudioClip works — import a .wav or .mp3 into the project and assign it here.")]
    [SerializeField] private AudioClip musicClip;

    [Range(0f, 1f)]
    [Tooltip("Overall music volume.")]
    [SerializeField] private float musicVolume = 0.5f;

    [Header("Audio Mixer Group")]
    [Tooltip("Assign the Music group from SilverValkyrieAudioMixer.")]
    [SerializeField] private AudioMixerGroup musicGroup;

    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        musicSource                    = gameObject.AddComponent<AudioSource>();
        musicSource.spatialBlend       = 0f;
        musicSource.playOnAwake        = false;
        musicSource.loop               = true;
        musicSource.volume             = musicVolume;
        musicSource.outputAudioMixerGroup = musicGroup;

        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Swaps the current music track and begins playing immediately.</summary>
    public void Play(AudioClip clip, float volume = -1f)
    {
        musicSource.Stop();
        musicSource.clip   = clip;
        musicSource.volume = volume >= 0f ? volume : musicVolume;
        musicSource.Play();
    }

    /// <summary>Pauses the current track at its current position.</summary>
    public void Pause() => musicSource.Pause();

    /// <summary>Resumes the track from where it was paused.</summary>
    public void Resume() => musicSource.UnPause();

    /// <summary>Sets the volume without stopping the track.</summary>
    public void SetVolume(float volume) => musicSource.volume = musicVolume = Mathf.Clamp01(volume);
}
