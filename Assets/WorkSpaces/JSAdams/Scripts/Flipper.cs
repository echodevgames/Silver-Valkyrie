using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Physics-driven pinball flipper. Attach to the pivot point of the flipper bat.
/// Uses MoveRotation on a Kinematic Rigidbody2D so that the implied angular velocity
/// correctly imparts force to the ball on contact.
///
/// For a right-side flipper, set the GameObject's X scale to -1 — the same
/// rest/active angles work for both sides without any script changes.
/// Input is sourced directly from SilverValkyrieInput; set Side in the Inspector.
/// </summary>
public class Flipper : MonoBehaviour
{
    public enum Side { Left, Right }

    [Header("Setup")]
    [SerializeField] private Side side;

    [Header("Angles")]
    [Tooltip("Z rotation (degrees) when the flipper is at rest (down position).")]
    [SerializeField] private float restAngle = -30f;
    [Tooltip("Z rotation (degrees) when the flipper is fully engaged (up position).")]
    [SerializeField] private float activeAngle = 30f;

    [Header("Speed")]
    [Tooltip("Degrees per second when engaging. Keep high — simulates a solenoid strike.")]
    [SerializeField] private float engageSpeed = 1800f;
    [Tooltip("Degrees per second when returning to rest.")]
    [SerializeField] private float returnSpeed = 720f;

    private Rigidbody2D rb;
    private SilverValkyrieInput input;
    private InputAction flipAction;
    private bool isPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        rb.MoveRotation(EffectiveAngle(restAngle));

        input = new SilverValkyrieInput();
        flipAction = side == Side.Left
            ? input.Gameplay.LeftFlipper
            : input.Gameplay.RightFlipper;
    }

    private void OnEnable()
    {
        input.Gameplay.Enable();
        flipAction.performed += OnPressed;
        flipAction.canceled  += OnReleased;
    }

    private void OnDisable()
    {
        flipAction.performed -= OnPressed;
        flipAction.canceled  -= OnReleased;
        input.Gameplay.Disable();
    }

    private void OnDestroy()
    {
        input.Dispose();
    }

    /// <summary>Called when the flip button is pressed.</summary>
    private void OnPressed(InputAction.CallbackContext ctx) => isPressed = true;

    /// <summary>Called when the flip button is released.</summary>
    private void OnReleased(InputAction.CallbackContext ctx) => isPressed = false;

    private void FixedUpdate()
    {
        float target = EffectiveAngle(isPressed ? activeAngle : restAngle);
        float speed  = isPressed ? engageSpeed : returnSpeed;
        float next   = Mathf.MoveTowards(rb.rotation, target, speed * Time.fixedDeltaTime);
        rb.MoveRotation(next);
    }

    // Angles are defined as "left-side" values. On the right flipper, scale.x = -1
    // visually inverts the rotation direction, so we negate to compensate.
    private float EffectiveAngle(float angle) => side == Side.Right ? -angle : angle;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        DrawAngleRay(restAngle,   Color.yellow);
        DrawAngleRay(activeAngle, Color.cyan);
    }

    /// <summary>Draws a ray from the pivot showing the given angle, respecting X scale for right flippers.</summary>
    private void DrawAngleRay(float degrees, Color color)
    {
        float rad   = degrees * Mathf.Deg2Rad;
        float xSign = Mathf.Sign(transform.lossyScale.x);
        Vector3 dir = new Vector3(Mathf.Cos(rad) * xSign, Mathf.Sin(rad), 0f);
        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawLine(transform.position, transform.position + dir * 2f);
    }
#endif
}