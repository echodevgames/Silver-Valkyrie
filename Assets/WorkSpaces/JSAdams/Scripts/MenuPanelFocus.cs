using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Attach to any menu panel root. Manages keyboard, gamepad and mouse focus for the panel:
///
///  - Gamepad / keyboard already active when panel opens → first Selectable auto-selected.
///  - Mouse active when panel opens → no selection; hover works normally.
///  - Tab / Shift+Tab → cycles through active, interactable Selectables in the panel.
///    First Tab press enters keyboard mode from mouse without skipping the first button.
///  - Arrow keys → handled natively by Unity's EventSystem once something is selected.
///  - Switching back to mouse mid-navigation → selection clears so hover takes over.
/// </summary>
[DisallowMultipleComponent]
public class MenuPanelFocus : MonoBehaviour
{
    [Tooltip("The Selectable that should receive focus when this panel opens on gamepad/keyboard.")]
    [SerializeField] private Selectable firstSelected;

    private void OnEnable()
    {
        InputDeviceTracker.OnDeviceChanged += HandleDeviceChanged;

        // Auto-select on open only for gamepad/keyboard — mouse users rely on hover.
        if (EventSystem.current == null || firstSelected == null) return;
        if (InputDeviceTracker.IsMouseOrTouchActive) return;

        EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
    }

    private void OnDisable()
    {
        InputDeviceTracker.OnDeviceChanged -= HandleDeviceChanged;

        // Clear selection so no stale highlight lingers on the hidden panel.
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject != null &&
            EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform))
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.tabKey.wasPressedThisFrame) return;

        CycleSelection(reverse: Keyboard.current.shiftKey.isPressed);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    /// <summary>
    /// Tab / Shift+Tab: if nothing is selected on this panel, selects firstSelected.
    /// If something is already selected, advances to the next (or previous) Selectable.
    /// </summary>
    private void CycleSelection(bool reverse)
    {
        if (EventSystem.current == null) return;

        // Gather all active, interactable Selectables that belong to this panel.
        var selectables = new List<Selectable>();
        foreach (var s in GetComponentsInChildren<Selectable>(false))
            if (s.interactable) selectables.Add(s);

        if (selectables.Count == 0) return;

        var currentGo  = EventSystem.current.currentSelectedGameObject;
        int currentIdx = currentGo != null
            ? selectables.FindIndex(s => s.gameObject == currentGo)
            : -1;

        if (currentIdx < 0)
        {
            // Nothing selected on this panel — Tab enters keyboard mode at the first button.
            int target = reverse ? selectables.Count - 1 : 0;
            EventSystem.current.SetSelectedGameObject(selectables[target].gameObject);
        }
        else
        {
            // Already navigating — advance with wrapping.
            int next = (currentIdx + (reverse ? -1 : 1) + selectables.Count) % selectables.Count;
            EventSystem.current.SetSelectedGameObject(selectables[next].gameObject);
        }
    }

    /// <summary>Reacts to live device switches while this panel is already open.</summary>
    private void HandleDeviceChanged(bool isMouseOrTouch)
    {
        if (EventSystem.current == null) return;

        // Only act on mouse switch — Tab and gamepad entry are handled by Update/OnEnable.
        if (!isMouseOrTouch) return;

        // Switched to mouse — drop the programmatic selection so hover takes over.
        if (EventSystem.current.currentSelectedGameObject != null &&
            EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform))
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
