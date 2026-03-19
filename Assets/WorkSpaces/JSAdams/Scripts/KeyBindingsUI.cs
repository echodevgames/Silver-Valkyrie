using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Full input rebinding overlay. Shows a grid: one row per bindable action,
/// three columns for Primary Keyboard, Alt Keyboard, and Gamepad.
///
/// Each <see cref="BindingRow"/> is wired in the Inspector — add a row entry and
/// point its six Button/Label fields at scene objects to expose a new action.
///
/// Relies on <see cref="InputService"/> as the single source of truth; overrides
/// are saved via <see cref="InputService.SaveOverrides"/> after every successful rebind.
/// </summary>
public class KeyBindingsUI : MonoBehaviour
{
    // ── Serializable row descriptor ───────────────────────────────────────────

    [Serializable]
    public struct BindingRow
    {
        [Tooltip("Action map name, e.g. 'Gameplay' or 'UI'.")]
        public string mapName;
        [Tooltip("Action name within that map, e.g. 'LeftFlipper'.")]
        public string actionName;

        [Header("Primary Keyboard (occurrence 0, Keyboard&Mouse)")]
        public Button            primaryButton;
        public TextMeshProUGUI   primaryLabel;

        [Header("Alt Keyboard (occurrence 1, Keyboard&Mouse)")]
        public Button            altButton;
        public TextMeshProUGUI   altLabel;

        [Header("Gamepad (occurrence 0, Gamepad)")]
        public Button            gamepadButton;
        public TextMeshProUGUI   gamepadLabel;
    }

    // ── Constants ─────────────────────────────────────────────────────────────

    private const string GroupKeyboard = "Keyboard&Mouse";
    private const string GroupGamepad  = "Gamepad";

    // ── Inspector fields ──────────────────────────────────────────────────────

    [SerializeField] private GameObject      panel;
    [SerializeField] private BindingRow[]    rows;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI conflictLabel;
    [SerializeField] private GameObject      listeningOverlay;
    [SerializeField] private TextMeshProUGUI listeningText;

    // ── State ─────────────────────────────────────────────────────────────────

    private GameObject _caller;
    private InputActionRebindingExtensions.RebindingOperation _rebindOp;
    private Coroutine _conflictDismiss;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (panel != null)            panel.SetActive(false);
        if (listeningOverlay != null) listeningOverlay.SetActive(false);

        if (conflictLabel != null)
        {
            conflictLabel.color = new Color(0.95f, 0.60f, 0.10f, 1f);
            conflictLabel.gameObject.SetActive(false);
        }
    }

    private void OnDisable() => CancelRebind();

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Shows the key bindings panel, hiding the caller.</summary>
    public void Show(GameObject caller)
    {
        _caller = caller;
        if (_caller != null) _caller.SetActive(false);

        WireButtonCallbacks();
        RefreshAllLabels();
        panel?.SetActive(true);
    }

    /// <summary>Closes the panel and restores the caller.</summary>
    public void OnClose()
    {
        CancelRebind();
        panel?.SetActive(false);
        if (_caller != null) _caller.SetActive(true);
        _caller = null;
    }

    /// <summary>Cancels any rebind and tears down the panel without restoring the caller. Used when the entire pause stack is force-dismissed.</summary>
    public void ForceClose()
    {
        CancelRebind();
        panel?.SetActive(false);
        _caller = null;
    }

    /// <summary>Resets all bindings to default and refreshes the display.</summary>
    public void OnResetAll()
    {
        InputService.Instance?.ResetOverrides();
        RefreshAllLabels();
    }

    /// <summary>Cancels any in-progress rebind without applying it.</summary>
    public void CancelRebind()
    {
        _rebindOp?.Cancel();
        _rebindOp?.Dispose();
        _rebindOp = null;
        if (listeningOverlay != null) listeningOverlay.SetActive(false);
    }

    // ── Button wiring ─────────────────────────────────────────────────────────

    private void WireButtonCallbacks()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            int captured = i; // closure capture

            AddClick(rows[i].primaryButton, () => StartRebind(captured, 0));
            AddClick(rows[i].altButton,     () => StartRebind(captured, 1));
            AddClick(rows[i].gamepadButton, () => StartRebind(captured, 2));
        }
    }

    private static void AddClick(Button btn, UnityEngine.Events.UnityAction action)
    {
        if (btn == null) return;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }

    // ── Label refresh ─────────────────────────────────────────────────────────

    private void RefreshAllLabels()
    {
        if (InputService.Instance == null) return;

        for (int i = 0; i < rows.Length; i++)
            RefreshRowLabels(i);
    }

    private void RefreshRowLabels(int rowIndex)
    {
        var row    = rows[rowIndex];
        var action = InputService.Instance.Asset.FindAction($"{row.mapName}/{row.actionName}");
        if (action == null) return;

        SetButtonText(row.primaryButton,  row.primaryLabel,  action, GroupKeyboard, occurrence: 0);
        SetButtonText(row.altButton,      row.altLabel,      action, GroupKeyboard, occurrence: 1);
        SetButtonText(row.gamepadButton,  row.gamepadLabel,  action, GroupGamepad,  occurrence: 0);
    }

    private static void SetButtonText(
        Button btn, TextMeshProUGUI label,
        InputAction action, string group, int occurrence)
    {
        int idx = FindBindingIndex(action, group, occurrence);

        if (label != null)
        {
            label.text = idx >= 0
                ? InputControlPath.ToHumanReadableString(
                    action.bindings[idx].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice)
                : "—";
        }

        if (btn != null)
            btn.interactable = idx >= 0;
    }

    // ── Rebinding ─────────────────────────────────────────────────────────────

    private void StartRebind(int rowIndex, int groupIndex)
    {
        if (InputService.Instance == null) return;

        var row    = rows[rowIndex];
        var action = InputService.Instance.Asset.FindAction($"{row.mapName}/{row.actionName}");
        if (action == null) return;

        string group = groupIndex == 2 ? GroupGamepad : GroupKeyboard;
        int    occ   = groupIndex == 1 ? 1 : 0;
        int    idx   = FindBindingIndex(action, group, occ);
        if (idx < 0) return;

        StartCoroutine(DoRebind(action, idx, rowIndex));
    }

    private IEnumerator DoRebind(InputAction action, int bindingIndex, int rowIndex)
    {
        // Yield one frame so the button-click that triggered this isn't captured.
        yield return null;

        string previousOverride = action.bindings[bindingIndex].overridePath;
        action.Disable();

        if (listeningOverlay != null) listeningOverlay.SetActive(true);
        if (listeningText    != null) listeningText.text = "Press a key or button…";

        _rebindOp = action
            .PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Mouse>")
            .OnComplete(op =>
            {
                op.Dispose();
                _rebindOp = null;
                action.Enable();

                if (listeningOverlay != null) listeningOverlay.SetActive(false);

                // Check for conflicts across all action maps.
                string conflict = FindConflict(action, bindingIndex);
                if (conflict != null)
                {
                    // Revert to the previous override.
                    if (previousOverride != null)
                        action.ApplyBindingOverride(bindingIndex, previousOverride);
                    else
                        action.RemoveBindingOverride(bindingIndex);

                    ShowConflict(conflict);
                }
                else
                {
                    InputService.Instance?.SaveOverrides();
                }

                RefreshRowLabels(rowIndex);
            })
            .OnCancel(op =>
            {
                op.Dispose();
                _rebindOp = null;
                action.Enable();
                if (listeningOverlay != null) listeningOverlay.SetActive(false);
                RefreshRowLabels(rowIndex);
            })
            .Start();
    }

    // ── Conflict detection ────────────────────────────────────────────────────

    /// <summary>
    /// Scans all action maps for another binding that uses the same effective path
    /// and belongs to the same control group. Returns the conflicting action name or null.
    /// </summary>
    private string FindConflict(InputAction changedAction, int bindingIndex)
    {
        if (InputService.Instance == null) return null;

        string newPath    = changedAction.bindings[bindingIndex].effectivePath;
        string groups     = changedAction.bindings[bindingIndex].groups;

        foreach (var map in InputService.Instance.Asset.actionMaps)
        {
            foreach (var action in map.actions)
            {
                if (action == changedAction) continue;

                foreach (var binding in action.bindings)
                {
                    if (binding.isComposite || binding.isPartOfComposite) continue;
                    if (string.IsNullOrEmpty(binding.groups))              continue;

                    // Only flag if they share at least one group.
                    bool sameGroup = false;
                    foreach (var g in groups.Split(';'))
                    {
                        if (binding.groups.Contains(g.Trim())) { sameGroup = true; break; }
                    }

                    if (sameGroup && binding.effectivePath == newPath)
                        return $"{action.name} ({map.name})";
                }
            }
        }

        return null;
    }

    private void ShowConflict(string withAction)
    {
        if (conflictLabel == null) return;
        conflictLabel.text = $"Conflict: already bound to {withAction}";
        conflictLabel.gameObject.SetActive(true);

        if (_conflictDismiss != null) StopCoroutine(_conflictDismiss);
        _conflictDismiss = StartCoroutine(DismissConflict());
    }

    private IEnumerator DismissConflict()
    {
        yield return new WaitForSecondsRealtime(3f);
        if (conflictLabel != null) conflictLabel.gameObject.SetActive(false);
    }

    // ── Utilities ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the index of the nth binding (occurrence) in the given group for the action.
    /// Returns -1 if no such slot exists.
    /// </summary>
    private static int FindBindingIndex(InputAction action, string group, int occurrence)
    {
        int count = 0;
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var b = action.bindings[i];
            if (b.isComposite || b.isPartOfComposite) continue;
            if (!b.groups.Contains(group))             continue;

            if (count == occurrence) return i;
            count++;
        }
        return -1;
    }
}
