using UnityEngine;
using UnityEngine.InputSystem;

public class Flipper : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Flipper Mechanics")]
    [SerializeField] private float flipSpeed = 2000f;
    [SerializeField] private float lowerAngle = -20f;
    [SerializeField] private float upperAngle = 35f;
    [SerializeField] private bool invertRotation;

    [Header("Input")]
    [SerializeField] private InputActionReference flipperAction;

    private bool isPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        flipperAction.action.Enable();
        flipperAction.action.performed += OnPressed;
        flipperAction.action.canceled += OnReleased;
    }

    private void OnDisable()
    {
        flipperAction.action.performed -= OnPressed;
        flipperAction.action.canceled -= OnReleased;
        flipperAction.action.Disable();
    }

    private void OnPressed(InputAction.CallbackContext ctx)
    {
        isPressed = true;
    }

    private void OnReleased(InputAction.CallbackContext ctx)
    {
        isPressed = false;
    }

    private void FixedUpdate()
    {
        float dir = invertRotation ? -1f : 1f;
        float target = isPressed ? upperAngle : lowerAngle;

        float newAngle = Mathf.MoveTowardsAngle(
            rb.rotation,
            target * dir,
            flipSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(newAngle);
    }
}