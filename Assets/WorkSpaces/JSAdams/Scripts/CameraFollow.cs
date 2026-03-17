using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Smoothly follows a target Transform while keeping the camera viewport
/// confined within the bounds of a reference Tilemap.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Confiner")]
    [Tooltip("Tilemap whose painted bounds define the camera limits.")]
    [SerializeField] private Tilemap confinerTilemap;

    [Header("Follow")]
    [Tooltip("Lerp speed. Higher = snappier.")]
    [SerializeField] private float followSpeed = 8f;

    private Camera cam;
    private Bounds worldBounds;
    private bool boundsReady;

    /// <summary>
    /// Applied on top of the clamped follow position each frame.
    /// Set by ScreenShakeService — zeroed automatically when the shake decays.
    /// </summary>
    public Vector3 ShakeOffset { get; set; }

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        RefreshBounds();
    }

    /// <summary>
    /// Recomputes the world-space bounds from the Tilemap.
    /// Call this if the tilemap is repainted at runtime.
    /// </summary>
    public void RefreshBounds()
    {
        if (confinerTilemap == null) return;

        // Tighten the bounds to only the actually painted tiles.
        confinerTilemap.CompressBounds();

        // Convert min/max from tilemap local space to world space.
        Bounds local = confinerTilemap.localBounds;
        Vector3 worldMin = confinerTilemap.transform.TransformPoint(local.min);
        Vector3 worldMax = confinerTilemap.transform.TransformPoint(local.max);
        worldBounds = new Bounds();
        worldBounds.SetMinMax(worldMin, worldMax);
        boundsReady = true;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position;
        desired.z = transform.position.z; // never move the camera in Z

        if (boundsReady)
            desired = ClampToConfiner(desired);

        transform.position = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime) + ShakeOffset;
    }

    /// <summary>
    /// Clamps the proposed camera center so the viewport edges stay inside worldBounds.
    /// If the tilemap is narrower/shorter than the viewport, it centers on the tilemap axis.
    /// </summary>
    private Vector3 ClampToConfiner(Vector3 pos)
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float minX = worldBounds.min.x + halfW;
        float maxX = worldBounds.max.x - halfW;
        float minY = worldBounds.min.y + halfH;
        float maxY = worldBounds.max.y - halfH;

        pos.x = minX > maxX ? worldBounds.center.x : Mathf.Clamp(pos.x, minX, maxX);
        pos.y = minY > maxY ? worldBounds.center.y : Mathf.Clamp(pos.y, minY, maxY);

        return pos;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!boundsReady || cam == null) return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        // Yellow = full tilemap bounds.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);

        // Cyan = safe zone the camera center is clamped within.
        Gizmos.color = Color.cyan;
        float safeW = Mathf.Max(0f, worldBounds.size.x - halfW * 2f);
        float safeH = Mathf.Max(0f, worldBounds.size.y - halfH * 2f);
        Gizmos.DrawWireCube(worldBounds.center, new Vector3(safeW, safeH, 0f));
    }
#endif
}
