// ----- BallState.cs START-----
using UnityEngine;

public class BallState : MonoBehaviour
{
    public System.Action<State> OnStateChanged;
    public enum State
    {
        InTrough,
        InPlunger,
        InPlayfield,
        Drained
    }

    public State CurrentState { get; private set; }

    public void SetState(State newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
// ----- BallState.CS END ----