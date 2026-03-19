using UnityEngine;

/// <summary>
/// Dynamically adjusts the main camera's viewport rectangle to reserve screen space
/// for the HUD side panel. Reads the panel's actual screen-space width each frame so
/// the adjustment is resolution-independent and works with any CanvasScaler mode.
///
/// Attach to the same GameObject as the Camera you want to constrain.
/// </summary>
[RequireComponent(typeof(Camera))]
public class HudCameraAdjuster : MonoBehaviour
{
    [Tooltip("RectTransform of the HUD panel anchored to the right edge of the screen.")]
    [SerializeField] private RectTransform hudPanel;

    private Camera _camera;
    private float  _lastViewportWidth = -1f;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (hudPanel == null) return;

        float screenWidth = Screen.width;
        if (screenWidth <= 0f) return;

        // Convert the panel's rect width from canvas space to screen-space pixels.
        float panelPixelWidth = hudPanel.rect.width * hudPanel.lossyScale.x;
        float viewportWidth   = Mathf.Clamp01(1f - panelPixelWidth / screenWidth);

        if (Mathf.Approximately(viewportWidth, _lastViewportWidth)) return;
        _lastViewportWidth = viewportWidth;

        Rect r   = _camera.rect;
        r.width  = viewportWidth;
        _camera.rect = r;
    }
}
