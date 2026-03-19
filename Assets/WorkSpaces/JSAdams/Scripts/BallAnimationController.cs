using UnityEngine;

/// <summary>
/// Drives the ball's Animator and transform rotation based on Rigidbody2D velocity.
///
/// Five effective directions — all derived from three animation clips:
///   Left       →  BallUp + z-rotation +90°
///   UpLeft     →  BallUpLeft
///   Up / Down  →  BallUp  (flipY for Down)
///   UpRight    →  BallUpRight
///   Right      →  BallUp + z-rotation -90°
///
/// flipY converts every Up variant into its Down counterpart.
/// CircleCollider2D is rotation-invariant so z-rotation is purely visual.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class BallAnimationController : MonoBehaviour
{
    private static readonly int DirectionParam = Animator.StringToHash("Direction");

    private const int DirUpLeft  = 0;
    private const int DirUp      = 1;
    private const int DirUpRight = 2;

    // Angle band (from ±X axis) that counts as moving sideways.
    // 150° means the horizontal zone spans ±30° off pure left/right.
    private const float SideAngle = 150f;

    [Tooltip("Minimum speed before the animator updates. Prevents jitter when the ball is nearly stationary.")]
    [SerializeField] private float speedThreshold = 1f;

    [Tooltip("Ball speed (units/s) at which the animation plays at 1× speed. Scale up if the animation still looks too slow at typical play speed.")]
    [SerializeField] private float referenceSpeed = 12f;

    private Rigidbody2D    rb;
    private Animator       animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb             = GetComponent<Rigidbody2D>();
        animator       = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;
        float   speed    = velocity.magnitude;

        // Freeze animation when nearly still; otherwise scale speed to clip rate
        animator.speed = speed < speedThreshold ? 0f : Mathf.Min(speed / referenceSpeed, 4f);

        if (speed < speedThreshold)
            return;

        // Signed angle from +X axis: 0=right, 90=up, ±180=left, -90=down
        float angle    = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        float absAngle = Mathf.Abs(angle);

        spriteRenderer.flipY = false;
        float zRotation = 0f;
        int   direction;

        if (absAngle > SideAngle)
        {
            direction = DirUp;
            zRotation = 90f;
        }
        else if (absAngle < 180f - SideAngle)
        {
            direction = DirUp;
            zRotation = -90f;
        }
        else
        {
            spriteRenderer.flipY = velocity.y < 0f;

            if (absAngle > 112.5f)
                direction = DirUpLeft;
            else if (absAngle > 67.5f)
                direction = DirUp;
            else
                direction = DirUpRight;
        }

        // SetRotation works with Rigidbody2D interpolation — avoids fighting the physics system
        rb.SetRotation(zRotation);
        animator.SetInteger(DirectionParam, direction);
    }
}
