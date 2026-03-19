using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Renders the leaderboard from HighScoreService and shows it as an overlay panel.
/// Call Show() to open (wired from PauseMenuUI.OnHighScores).
/// The Close button wires to OnClose() via Inspector PersistentCalls.
/// </summary>
public class HighScoreBoardUI : MonoBehaviour
{
    [SerializeField] private GameObject          panel;
    [SerializeField] private TextMeshProUGUI     boardText;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    /// <summary>Refreshes leaderboard text and shows the panel.</summary>
    public void Show()
    {
        RefreshBoard();
        panel?.SetActive(true);
    }

    /// <summary>Hides the panel. Wired to the Close button via PersistentCalls.</summary>
    public void OnClose()
    {
        panel?.SetActive(false);
    }

    private void RefreshBoard()
    {
        if (boardText == null) return;

        var entries = HighScoreService.Instance?.Entries;

        if (entries == null || entries.Count == 0)
        {
            boardText.text =
                "<color=#889098>No scores recorded yet.\n\n" +
                "Finish a game to get on the board.</color>";
            return;
        }

        var sb = new StringBuilder();

        for (int i = 0; i < entries.Count; i++)
        {
            var    e         = entries[i];
            string rankColor = i == 0 ? "#E8C840" : i < 3 ? "#5AB8D8" : "#ECE8D8";
            string rankLabel = $"{i + 1,2}.";

            sb.Append($"<color={rankColor}><b>{rankLabel}</b>  {e.score:N0}</color>");
            sb.AppendLine($"  <color=#889098><size=70%>{e.date}</size></color>");
        }

        boardText.text = sb.ToString().TrimEnd();
    }
}
