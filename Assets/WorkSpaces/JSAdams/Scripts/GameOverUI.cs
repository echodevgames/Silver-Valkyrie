using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Listens for BallLifeService.OnGameOver, submits the final score to HighScoreService,
/// and shows the game over panel with rank feedback, top 3 scores, and action buttons.
///
/// Continue button is enabled only when LivesService reports remaining continues.
/// Clicking it calls LivesService.UseContinue(), cancels the GameDirector freeze, and
/// spawns a new ball so play resumes immediately.
///
/// Button callbacks are wired via Inspector PersistentCalls — no AddListener required.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject       panel;
    [SerializeField] private TextMeshProUGUI  scoreText;
    [SerializeField] private TextMeshProUGUI  topScoresText;
    [SerializeField] private HighScoreBoardUI highScoreBoard;

    [Header("Continue")]
    [SerializeField] private Button           continueButton;
    [SerializeField] private TextMeshProUGUI  continueLabel;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void OnEnable()  => BallLifeService.OnGameOver += HandleGameOver;
    private void OnDisable() => BallLifeService.OnGameOver -= HandleGameOver;

    private void HandleGameOver()
    {
        int finalScore = ScoreService.Instance?.Score ?? 0;
        int rank       = HighScoreService.Instance?.Submit(finalScore) ?? -1;

        var sb = new StringBuilder();

        if (rank == 1)
            sb.Append("<color=#E8C840><b>— NEW HIGH SCORE —</b></color>\n\n");
        else if (rank > 0)
            sb.Append($"<color=#5AB8D8>RANK #{rank}</color>\n\n");

        sb.Append("<color=#889098><size=70%>FINAL SCORE</size></color>\n");
        sb.Append($"<color=#E8C840><b>{finalScore:N0}</b></color>");

        if (scoreText != null)
            scoreText.text = sb.ToString();

        RefreshTopScores();
        RefreshContinueButton();
        panel?.SetActive(true);
    }

    private void RefreshTopScores()
    {
        if (topScoresText == null) return;

        var entries = HighScoreService.Instance?.Entries;

        if (entries == null || entries.Count == 0)
        {
            topScoresText.text = "<color=#889098>No scores yet.</color>";
            return;
        }

        var sb    = new StringBuilder();
        int count = Mathf.Min(3, entries.Count);

        sb.Append("<color=#889098><size=70%>TOP SCORES</size></color>\n");

        for (int i = 0; i < count; i++)
        {
            string c = i == 0 ? "#E8C840" : "#5AB8D8";
            sb.Append($"<color={c}><b>{i + 1,2}.</b>  {entries[i].score:N0}</color>");
            if (i < count - 1) sb.Append("\n");
        }

        topScoresText.text = sb.ToString();
    }

    private void RefreshContinueButton()
    {
        if (continueButton == null) return;

        int lives = LivesService.Instance?.LivesRemaining ?? 0;
        continueButton.interactable = lives > 0;

        if (continueLabel != null)
            continueLabel.text = lives > 0 ? $"CONTINUE  ({lives})" : "CONTINUE";
    }

    // ── Button callbacks — wired via Inspector PersistentCalls ────────────────

    /// <summary>
    /// Spends one continue, cancels the game-over freeze, hides the panel, and spawns a ball.
    /// </summary>
    public void OnContinue()
    {
        bool success = LivesService.Instance?.UseContinue() ?? false;
        if (!success) return;

        Debug.Log("[GameOver] Continue used — resuming play.");
        GameDirector.Instance?.CancelGameOverFreeze();
        panel?.SetActive(false);
        GameDirector.Instance?.SpawnBall();
    }

    /// <summary>Resets time and audio, then reloads the scene via GameDirector.</summary>
    public void OnRestart()
    {
        Debug.Log("[GameOver] Restart");
        Time.timeScale      = 1f;
        AudioListener.pause = false;
        panel?.SetActive(false);
        GameDirector.Instance?.RestartGame();
    }

    /// <summary>Opens the high score board overlay and hides the game over panel behind it.</summary>
    public void OnViewHighScores()
    {
        Debug.Log("[GameOver] View High Scores");
        highScoreBoard?.Show(panel);
    }
}
