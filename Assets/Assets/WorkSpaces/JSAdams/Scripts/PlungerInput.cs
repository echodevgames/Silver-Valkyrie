
// -----PlungerInput.cs START-----
using UnityEngine;
using UnityEngine.InputSystem;

public class PlungerInput : MonoBehaviour
{
    [Header("Movement")]
    public float pullSpeed = 2f;
    public float returnSpeed = 16f;

    [Header("Charge")]
    public float maxCharge = 1.2f;        // seconds to fully charge
    public float minLaunchPower = 10f;
    public float maxLaunchPower = 40f;

    private Rigidbody2D rb;
    private Vector2 restPosition;

    private bool pulling;
    private float charge;                 // accumulates while holding

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        restPosition = rb.position;
    }

    void Update()
    {
        pulling = Keyboard.current.spaceKey.isPressed;

        if (pulling)
        {
            charge += Time.deltaTime;
            charge = Mathf.Clamp(charge, 0f, maxCharge);
        }
    }

    void FixedUpdate()
    {
        if (pulling)
        {
            rb.MovePosition(
                rb.position + Vector2.down * pullSpeed * Time.fixedDeltaTime
            );
        }
        else
        {
            Vector2 newPos = rb.position + Vector2.up * returnSpeed * Time.fixedDeltaTime;

            if (newPos.y > restPosition.y)
                newPos.y = restPosition.y;

            rb.MovePosition(newPos);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!pulling && collision.collider.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = collision.collider.GetComponent<Rigidbody2D>();

            float t = charge / maxCharge; // 0–1
            float launchPower = Mathf.Lerp(minLaunchPower, maxLaunchPower, t);

            ballRb.AddForce(Vector2.up * launchPower, ForceMode2D.Impulse);

            charge = 0f; // reset after launch
        }
    }
}


// -----PlungerInput.cs START-----
