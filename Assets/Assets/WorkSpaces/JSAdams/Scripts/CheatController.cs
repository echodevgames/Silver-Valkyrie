using UnityEngine;
using UnityEngine.InputSystem;

public class CheatController : MonoBehaviour
{
    [SerializeField] private BallManager ballManager;
    [SerializeField] private TroughGate troughGate;

    private void Update()
    {
        if (!Application.isEditor) return; // optional safety

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("CHEAT: Force Gate Open");
            troughGate.ForceOpen();
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("CHEAT: Force Gate Close");
            troughGate.ForceClose();
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Debug.Log("CHEAT: Multiball Mode");
            GameStateManager.Instance.SetModeMultiball();
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Debug.Log("CHEAT: Default Mode");
            GameStateManager.Instance.SetModeDefault();
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            Debug.Log("CHEAT: Drain Primary Ball");
            ballManager.DebugDrainPrimaryBall();
        }
    }
}