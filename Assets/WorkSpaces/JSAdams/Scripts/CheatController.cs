using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Developer cheat keys. Active in the Unity Editor and development builds only —
/// stripped from release builds via conditional compilation.
///
/// Keybindings:
///   B  — Spawn a ball at the plunger
///   K  — Kill (destroy) the first ball found in the scene
///   L  — Add one ball/life to the pool
///   R  — Restart (reload the scene immediately)
/// </summary>
public class CheatController : MonoBehaviour
{
    [SerializeField] private GameDirector gameDirector;

    private void Update()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        return;
#endif
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // B — spawn ball
        if (keyboard.bKey.wasPressedThisFrame)
        {
            Debug.Log("[CHEAT] Spawn ball");
            gameDirector.SpawnBall();
        }

        // K — kill active ball (counts as a drain — triggers respawn or game over)
        if (keyboard.kKey.wasPressedThisFrame)
        {
            GameObject ball = GameObject.FindWithTag("Ball");
            if (ball != null)
            {
                Debug.Log("[CHEAT] Kill ball");
                Destroy(ball);
                BallLifeService.Instance?.SimulateDrain();
            }
            else
            {
                Debug.Log("[CHEAT] No ball found.");
            }
        }

        // L — add a life
        if (keyboard.lKey.wasPressedThisFrame)
        {
            Debug.Log("[CHEAT] +1 ball/life");
            BallLifeService.Instance?.AddBalls(1);
        }

        // R — restart (also unfreeze in case game is in game over freeze state)
        if (keyboard.rKey.wasPressedThisFrame)
        {
            Debug.Log("[CHEAT] Restart");
            Time.timeScale = 1f;
            gameDirector.RestartGame();
        }
    }
}