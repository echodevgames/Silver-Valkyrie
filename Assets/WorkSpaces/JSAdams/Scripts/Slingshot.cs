using System.Collections;
using UnityEngine;

/// <summary>
/// Slingshot visual controller. On ball contact, flashes the hit sprite briefly then returns to idle.
/// All physics feedback (screen shake, controller vibration, scoring, audio) is handled
/// by the SurfaceContact component on the same GameObject.
///
/// The collision shape is defined by the PolygonCollider2D — adjust its points in the Inspector
/// to match the exact sprite outline. The left variant can be mirrored to produce the right variant
/// by duplicating the prefab and enabling flipX on the SpriteRenderer.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SurfaceContact))]
public class Slingshot : MonoBehaviour
{
    private const string BallTag = "Ball";

    [Header("Sprites")]
    [Tooltip("Sprite displayed at rest.")]
    [SerializeField] private Sprite idleSprite;

    [Tooltip("Sprite displayed briefly when the ball makes contact.")]
    [SerializeField] private Sprite hitSprite;

    [Header("Timing")]
    [Tooltip("How long (seconds) the hit sprite is displayed before returning to idle.")]
    [SerializeField] private float hitFlashDuration = 0.10f;

    // ── Private ───────────────────────────────────────────────────────────────

    private SpriteRenderer _renderer;
    private Coroutine      _flashRoutine;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        SetIdle();
    }

    // ── Collision ─────────────────────────────────────────────────────────────

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag(BallTag)) return;
        TriggerFlash();
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private void TriggerFlash()
    {
        if (_flashRoutine != null) StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        if (hitSprite != null) _renderer.sprite = hitSprite;
        yield return new WaitForSeconds(hitFlashDuration);
        SetIdle();
        _flashRoutine = null;
    }

    private void SetIdle()
    {
        if (idleSprite != null && _renderer != null)
            _renderer.sprite = idleSprite;
    }
}
