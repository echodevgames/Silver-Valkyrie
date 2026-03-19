using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// Lightweight singleton that tracks whether the player's most recent input came from
/// a mouse/touchscreen or from a keyboard/gamepad.
///
/// Add this component to a persistent GameObject in the scene (e.g. DevUI root).
/// <see cref="MenuPanelFocus"/> reads <see cref="IsMouseOrTouchActive"/> and subscribes
/// to <see cref="OnDeviceChanged"/> to manage UI selection automatically.
/// </summary>
[DisallowMultipleComponent]
public class InputDeviceTracker : MonoBehaviour
{
    public static InputDeviceTracker Instance { get; private set; }

    /// <summary>
    /// True when the most recent input event came from a mouse or touchscreen.
    /// Starts as true so menus open cleanly on desktop before any input is received.
    /// </summary>
    public static bool IsMouseOrTouchActive { get; private set; } = true;

    /// <summary>
    /// Fires whenever the active device category changes.
    /// Argument is true when switching to mouse/touch, false when switching to keyboard/gamepad.
    /// </summary>
    public static event Action<bool> OnDeviceChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()  => InputSystem.onEvent += HandleInputEvent;
    private void OnDisable() => InputSystem.onEvent -= HandleInputEvent;

    private static void HandleInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        bool wasMouse = IsMouseOrTouchActive;

        if (device is Mouse || device is Touchscreen)
            IsMouseOrTouchActive = true;
        else if (device is Keyboard || device is Gamepad)
            IsMouseOrTouchActive = false;

        if (IsMouseOrTouchActive != wasMouse)
            OnDeviceChanged?.Invoke(IsMouseOrTouchActive);
    }
}
