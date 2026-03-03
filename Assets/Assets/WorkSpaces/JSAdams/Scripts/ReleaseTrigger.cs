using UnityEngine;

public class ReleaseTrigger : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Ball")) return;

        BallState state = other.GetComponent<BallState>();
        if (state != null && state.CurrentState == BallState.State.InTrough)
        {
            state.SetState(BallState.State.InPlunger);
        }
    }
}