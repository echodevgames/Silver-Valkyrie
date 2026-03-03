using UnityEngine;

public class TroughBrakeZone : MonoBehaviour
{
    [SerializeField] private float brakeMultiplier = 0.85f; // 0.8–0.9 is ideal

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Ball")) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        // Only damp horizontal motion (toward gate)
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x * brakeMultiplier,
            rb.linearVelocity.y
        );
    }
}