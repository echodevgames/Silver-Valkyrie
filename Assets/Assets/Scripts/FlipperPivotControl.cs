using UnityEngine;

public class FlipperPivotControl : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode primaryKey = KeyCode.A;
    public KeyCode secondaryKey = KeyCode.LeftArrow;

    [Header("Rotation Settings")]
    public float restAngle = -45f;    // Will be overridden by editor angle
    public float flippedAngle = 0f;
    public float rotationSpeed = 400f;

    [Header("Physics Force Settings")]
    public float flipForce = 1500f;      // Force magnitude applied to ball
    public Transform flipperChild;      // Reference to the child flipper with colliders

    private float currentAngle;
    private Vector3 baseRotation; // Store the original rotation from editor
    private float previousAngle;
    private float flipperAngularVelocity;
    private Rigidbody ballInContact;    // Reference to ball currently touching flipper

    private void Start()
    {
        // Capture the full rotation from editor
        baseRotation = transform.localRotation.eulerAngles;

        // Grab only the Y rotation as the starting rest angle
        restAngle = baseRotation.y;
        currentAngle = restAngle;

        // Set up collision detection on the child flipper
        if (flipperChild != null)
        {
            FlipperCollisionHandler handler = flipperChild.GetComponent<FlipperCollisionHandler>();
            if (handler == null)
            {
                handler = flipperChild.gameObject.AddComponent<FlipperCollisionHandler>();
            }
            handler.parentFlipper = this;
        }
    }

    private void Update()
    {
        bool isPressed = Input.GetKey(primaryKey) || Input.GetKey(secondaryKey);
        float targetAngle = isPressed ? flippedAngle : restAngle;

        // Calculate flipper's angular velocity (how fast it's rotating)
        flipperAngularVelocity = (currentAngle - previousAngle) / Time.deltaTime;
        previousAngle = currentAngle;

        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        // Use base rotation but only modify the Y-axis
        transform.localRotation = Quaternion.Euler(baseRotation.x, currentAngle, baseRotation.z);
    }

    // Called by the collision handler on the flipper child
    public void OnChildTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ballInContact = other.GetComponent<Rigidbody>();
        }
    }

    public void OnChildTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball") && ballInContact != null)
        {
            // Apply force when ball leaves contact (if flipper was active)
            bool flipperIsActive = Input.GetKey(primaryKey) || Input.GetKey(secondaryKey);
            if (flipperIsActive)
            {
                ApplyFlipForce();
            }

            ballInContact = null;
        }
    }

    private void ApplyFlipForce()
    {
        if (ballInContact == null || flipperChild == null) return;

        // The force is based on how fast the flipper is moving
        float flipperSpeed = Mathf.Abs(flipperAngularVelocity);

        // Fast flipper movement = strong force, slow movement = weak force
        float appliedForce = flipperSpeed * (flipForce * 100);

        // Direction: flipper's current "up" direction (where the flipper surface points)
        Vector3 flipperSurfaceDirection = flipperChild.up;

        // Apply force based on flipper's actual movement speed
        ballInContact.AddForce(flipperSurfaceDirection * appliedForce, ForceMode.Impulse);
    }
}