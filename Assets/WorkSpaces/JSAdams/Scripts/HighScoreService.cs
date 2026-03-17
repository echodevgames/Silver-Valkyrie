using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HighScoreEntry
{
    public int    score;
    public string date;   // yyyy-MM-dd
}

[Serializable]
public class HighScoreData
{
    public List<HighScoreEntry> entries = new List<HighScoreEntry>();
}

/// <summary>
/// Persists the top N scores across sessions using PlayerPrefs + JsonUtility.
/// Call Submit(score) after a game ends — it returns the 1-based rank if the score
/// made the leaderboard, or -1 if it was too low or zero.
/// Access Entries for a read-only, sorted snapshot of the board.
/// </summary>
public class HighScoreService : MonoBehaviour
{
    public static HighScoreService Instance { get; private set; }

    [Tooltip("Maximum number of scores kept on the leaderboard.")]
    [SerializeField] private int maxEntries = 10;

    private const string SaveKey = "SV_HighScores";

    private HighScoreData _data = new HighScoreData();

    /// <summary>Read-only snapshot of the leaderboard, sorted highest-to-lowest.</summary>
    public IReadOnlyList<HighScoreEntry> Entries => _data.entries;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Load();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// Adds the score to the board, trims to maxEntries, and persists.
    /// Returns the 1-based rank if the score made the board, or -1 if it did not
    /// (score was zero, or it fell below the current bottom entry).
    /// </summary>
    public int Submit(int score)
    {
        if (score <= 0) return -1;

        var entry = new HighScoreEntry
        {
            score = score,
            date  = DateTime.Now.ToString("yyyy-MM-dd")
        };

        _data.entries.Add(entry);
        _data.entries.Sort((a, b) => b.score.CompareTo(a.score));

        // Capture rank before any trimming so we report the true position.
        int rank = _data.entries.IndexOf(entry) + 1; // 1-based

        if (_data.entries.Count > maxEntries)
            _data.entries.RemoveRange(maxEntries, _data.entries.Count - maxEntries);

        Save();

        Debug.Log($"[HighScoreService] Score {score:N0} submitted — rank #{rank}. Board: {_data.entries.Count} entries.");
        return rank <= maxEntries ? rank : -1;
    }

    /// <summary>Erases all saved scores from memory and PlayerPrefs.</summary>
    public void Clear()
    {
        _data = new HighScoreData();
        Save();
        Debug.Log("[HighScoreService] Leaderboard cleared.");
    }

    private void Save()
    {
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(_data));
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return;

        _data = JsonUtility.FromJson<HighScoreData>(PlayerPrefs.GetString(SaveKey))
                ?? new HighScoreData();

        Debug.Log($"[HighScoreService] Loaded {_data.entries.Count} saved score(s).");
    }
}
