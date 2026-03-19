using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Developer cheat input handler. Active in the Unity Editor and development builds only.
///
/// Driven by the "Cheats" action map in SilverValkyrieInput when it exists.
/// Falls back to direct keyboard polling if the map has not yet been added to the asset.
///
/// Keybindings (default):
///   B  — Spawn a ball at the plunger
///   K  — Kill (destroy) the first ball found — counts as a drain
///   L  — Add one ball/life to the pool
///   H  — Toggle cheat sheet overlay
///   R  — Restart (reload the scene, unfreeze time and audio)
/// </summary>
public class CheatController : MonoBehaviour
{
    [SerializeField] private GameDirector gameDirector;

    private InputActionMap _cheatsMap;
    private InputAction    _spawnBall;
    private InputAction    _killBall;
    private InputAction    _addLife;
    private InputAction    _toggleCheatSheet;
    private InputAction    _restart;

    private void Awake()
    {
        var asset = new SilverValkyrieInput().asset;
        _cheatsMap = asset.FindActionMap("Cheats");

        if (_cheatsMap != null)
        {
            _spawnBall        = _cheatsMap.FindAction("SpawnBall");
            _killBall         = _cheatsMap.FindAction("KillBall");
            _addLife          = _cheatsMap.FindAction("AddLife");
            _toggleCheatSheet = _cheatsMap.FindAction("ToggleCheatSheet");
            _restart          = _cheatsMap.FindAction("Restart");
        }
        else
        {
            Debug.LogWarning("[CheatController] 'Cheats' action map not found — falling back to keyboard polling. " +
                             "Add it in the Input Actions editor to enable action-based cheat input.");
        }
    }

    private void OnEnable()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (_cheatsMap == null) return;
        _spawnBall.performed        += OnSpawnBall;
        _killBall.performed         += OnKillBall;
        _addLife.performed          += OnAddLife;
        _toggleCheatSheet.performed += OnToggleCheatSheet;
        _restart.performed          += OnRestart;
        _cheatsMap.Enable();
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (_cheatsMap == null) return;
        _spawnBall.performed        -= OnSpawnBall;
        _killBall.performed         -= OnKillBall;
        _addLife.performed          -= OnAddLife;
        _toggleCheatSheet.performed -= OnToggleCheatSheet;
        _restart.performed          -= OnRestart;
        _cheatsMap.Disable();
#endif
    }

    private void Update()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        return;
#endif
        if (_cheatsMap != null) return; // action map has taken over

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.bKey.wasPressedThisFrame) OnSpawnBall();
        if (keyboard.kKey.wasPressedThisFrame) OnKillBall();
        if (keyboard.lKey.wasPressedThisFrame) OnAddLife();
        if (keyboard.hKey.wasPressedThisFrame) OnToggleCheatSheet();
        if (keyboard.rKey.wasPressedThisFrame) OnRestart();
    }

    // ── Cheat implementations ─────────────────────────────────────────────────

    private void OnSpawnBall(InputAction.CallbackContext _ = default)
    {
        Debug.Log("[CHEAT] Spawn ball");
        gameDirector.SpawnBall();
    }

    private void OnKillBall(InputAction.CallbackContext _ = default)
    {
        var ball = GameObject.FindWithTag("Ball");
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

    private void OnAddLife(InputAction.CallbackContext _ = default)
    {
        Debug.Log("[CHEAT] +1 ball/life");
        BallLifeService.Instance?.AddBalls(1);
    }

    private void OnToggleCheatSheet(InputAction.CallbackContext _ = default)
    {
        Debug.Log("[CHEAT] Toggle cheat sheet");
        FindFirstObjectByType<CheatSheetUI>()?.Toggle();
    }

    private void OnRestart(InputAction.CallbackContext _ = default)
    {
        Debug.Log("[CHEAT] Restart");
        Time.timeScale      = 1f;
        AudioListener.pause = false;
        gameDirector.RestartGame();
    }
}