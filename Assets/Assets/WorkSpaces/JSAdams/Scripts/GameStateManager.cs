
// ----- GameStateManager.cs START -----

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GameStateManager : MonoBehaviour
{

    [SerializeField] private BallManager ballManager;
    [SerializeField] private StartUIController startUI;
    public static GameStateManager Instance { get; private set; }

    private bool gameStarted = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        PauseGame();
    }

    private void Update()
    {
        // Press Any Key to Start
        if (!gameStarted &&
            (
                Keyboard.current.anyKey.wasPressedThisFrame ||
                (Gamepad.current != null && Gamepad.current.allControls.Any(c => c.IsPressed()))
            ))
        {
            BeginGameSequence();
        }

        // ESC to Quit
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            QuitGame();
        }
    }

    private void BeginGameSequence()
    {
        if (gameStarted) return;

        gameStarted = true;
        StartCoroutine(GameStartRoutine());
    }


    private IEnumerator GameStartRoutine()
    {
        startUI.StopBlinking();

        yield return startUI.PlayCountdown();

        Time.timeScale = 1f;

        if (ballManager != null)
            ballManager.StartSpawning();

        Debug.Log("Game Started");
    }
    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void QuitGame()
    {
        Debug.Log("Quit Game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

// ----- GameStateManager.cs END -----