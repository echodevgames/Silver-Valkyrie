// ----- Drain.cs START -----

using UnityEngine;

public class Drain : MonoBehaviour
{
    [SerializeField] private BallManager ballManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Make sure we only drain actual balls (recommended: tag your ball prefab "Ball")
        if (!other.CompareTag("Ball"))
            return;

        ballManager.BallDrained(other.gameObject);
    }
}

// ----- Drain.cs END -----