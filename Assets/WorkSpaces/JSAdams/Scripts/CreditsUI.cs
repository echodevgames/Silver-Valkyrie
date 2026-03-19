using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the credits overlay panel. Uses the caller-swap pattern: Show(caller) hides
/// the calling panel and reveals credits; OnClose() reverses the swap.
/// Add new team members by extending the Roles array — no scene edits required.
/// </summary>
public class CreditsUI : MonoBehaviour
{
    private readonly struct CreditEntry
    {
        public readonly string Role;
        public readonly string Name;
        public CreditEntry(string role, string name) { Role = role; Name = name; }
    }

    private static readonly CreditEntry[] Roles =
    {
        new("DESIGN & CODE",  "J.S. Adams"),
        new("ART",            "JD"),
        new("SOUND & MUSIC",  "M. Cintra"),
    };

    // ── Inspector fields ──────────────────────────────────────────────────────

    [SerializeField] private GameObject      panel;
    [SerializeField] private TextMeshProUGUI creditsText;

    // ── State ─────────────────────────────────────────────────────────────────

    private GameObject _caller;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Shows the credits panel and hides the calling panel (e.g. pause menu).</summary>
    public void Show(GameObject caller)
    {
        _caller = caller;
        if (_caller != null) _caller.SetActive(false);
        BuildCreditsText();
        panel?.SetActive(true);
    }

    /// <summary>Hides credits and restores the caller panel.</summary>
    public void OnClose()
    {
        panel?.SetActive(false);
        if (_caller != null) _caller.SetActive(true);
        _caller = null;
    }

    /// <summary>Tears down the panel without restoring the caller. Used when the entire pause stack is force-dismissed.</summary>
    public void ForceClose()
    {
        panel?.SetActive(false);
        _caller = null;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void BuildCreditsText()
    {
        if (creditsText == null) return;

        var sb = new StringBuilder();
        sb.AppendLine("<color=#E8C740><b>SILVER VALKYRIE</b></color>");
        sb.AppendLine();

        foreach (CreditEntry entry in Roles)
        {
            sb.AppendLine($"<color=#8888AA><size=80%>{entry.Role}</size></color>");
            sb.AppendLine($"<color=#EEE8CC>{entry.Name}</color>");
            sb.AppendLine();
        }

        creditsText.text = sb.ToString().TrimEnd();
    }
}
