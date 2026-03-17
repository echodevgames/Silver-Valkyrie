using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor window for recording audio from a connected microphone and saving
/// the result as a .wav asset in the project.
/// Open via Silver Valkyrie > Microphone Recorder.
/// </summary>
public class MicrophoneRecorderWindow : EditorWindow
{
    private const int MaxRecordingSeconds = 10;
    private const int RecordingFrequency  = 44100;
    private const string DefaultSavePath  = "Assets/WorkSpaces/JSAdams/Audio";

    private string[]    devices;
    private int         selectedDevice;
    private AudioClip   recordedClip;
    private bool        isRecording;
    private float       recordingStartTime;
    private string      clipName = "NewSound";
    private AudioSource previewSource;

    [MenuItem("Silver Valkyrie/Microphone Recorder")]
    public static void Open() => GetWindow<MicrophoneRecorderWindow>("Mic Recorder");

    private void OnEnable()
    {
        devices = Microphone.devices;
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Microphone Recorder", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        // Device selection
        if (devices == null || devices.Length == 0)
        {
            EditorGUILayout.HelpBox("No microphone devices found.", MessageType.Warning);
            return;
        }

        selectedDevice = EditorGUILayout.Popup("Device", selectedDevice, devices);
        clipName       = EditorGUILayout.TextField("Clip Name", clipName);

        EditorGUILayout.Space(6);

        // Recording controls
        using (new EditorGUI.DisabledGroupScope(isRecording))
        {
            if (GUILayout.Button("⏺  Record", GUILayout.Height(32)))
                StartRecording();
        }

        using (new EditorGUI.DisabledGroupScope(!isRecording))
        {
            if (GUILayout.Button("⏹  Stop", GUILayout.Height(32)))
                StopRecording();
        }

        // Progress bar while recording
        if (isRecording)
        {
            float elapsed  = Time.realtimeSinceStartup - recordingStartTime;
            float progress = Mathf.Clamp01(elapsed / MaxRecordingSeconds);
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(false, 18), progress,
                $"{elapsed:F1}s / {MaxRecordingSeconds}s");
            Repaint();
        }

        EditorGUILayout.Space(6);

        // Preview and save
        using (new EditorGUI.DisabledGroupScope(recordedClip == null || isRecording))
        {
            if (GUILayout.Button("▶  Preview"))
                PreviewClip();

            EditorGUILayout.Space(4);

            if (GUILayout.Button("💾  Save as WAV Asset", GUILayout.Height(32)))
                SaveClip();
        }

        if (recordedClip != null && !isRecording)
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.HelpBox(
                $"Recorded: {recordedClip.length:F2}s — assign it to a Surface Material's Hit Sound field.",
                MessageType.Info);
        }
    }

    private void StartRecording()
    {
        string device      = devices[selectedDevice];
        recordedClip       = Microphone.Start(device, false, MaxRecordingSeconds, RecordingFrequency);
        isRecording        = true;
        recordingStartTime = Time.realtimeSinceStartup;
    }

    private void StopRecording()
    {
        string device   = devices[selectedDevice];
        int    position = Microphone.GetPosition(device);
        Microphone.End(device);
        isRecording = false;

        if (position > 0)
            recordedClip = TrimClip(recordedClip, position);
    }

    private void PreviewClip()
    {
        if (previewSource == null)
        {
            GameObject go = EditorUtility.CreateGameObjectWithHideFlags(
                "MicPreview", HideFlags.HideAndDontSave, typeof(AudioSource));
            previewSource = go.GetComponent<AudioSource>();
        }

        previewSource.clip = recordedClip;
        previewSource.Play();
    }

    private void SaveClip()
    {
        if (recordedClip == null) return;

        string directory = DefaultSavePath;
        if (!AssetDatabase.IsValidFolder(directory))
            Directory.CreateDirectory(directory);

        string safeName = string.IsNullOrWhiteSpace(clipName) ? "NewSound" : clipName;
        string fullPath = Path.Combine(directory, $"{safeName}.wav");

        byte[] wav = EncodeToWav(recordedClip);
        File.WriteAllBytes(fullPath, wav);

        AssetDatabase.ImportAsset(fullPath);
        Debug.Log($"[MicRecorder] Saved → {fullPath}");

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<AudioClip>(fullPath);
    }

    private void OnDestroy()
    {
        if (devices != null && devices.Length > 0 && Microphone.IsRecording(devices[selectedDevice]))
            Microphone.End(devices[selectedDevice]);

        if (previewSource != null)
            DestroyImmediate(previewSource.gameObject);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Trims a recorded AudioClip to the actual number of captured samples.</summary>
    private static AudioClip TrimClip(AudioClip source, int samplePosition)
    {
        float[] data = new float[samplePosition * source.channels];
        source.GetData(data, 0);

        AudioClip trimmed = AudioClip.Create(source.name, samplePosition, source.channels, source.frequency, false);
        trimmed.SetData(data, 0);
        return trimmed;
    }

    /// <summary>Encodes an AudioClip to raw PCM WAV bytes (16-bit, little-endian).</summary>
    private static byte[] EncodeToWav(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        short[] pcm = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
            pcm[i] = (short)(Mathf.Clamp(samples[i], -1f, 1f) * short.MaxValue);

        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);

        int dataSize   = pcm.Length * 2;
        int sampleRate = clip.frequency;
        int channels   = clip.channels;

        // RIFF header
        writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(36 + dataSize);
        writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
        // fmt chunk
        writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);                  // PCM
        writer.Write((short)channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * 2); // byte rate
        writer.Write((short)(channels * 2));     // block align
        writer.Write((short)16);                 // bits per sample
        // data chunk
        writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
        writer.Write(dataSize);
        foreach (short s in pcm) writer.Write(s);

        return stream.ToArray();
    }
}
