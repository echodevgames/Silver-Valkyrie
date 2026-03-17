using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Reacts to BallLifeService events to handle ball respawn and game over / restart flow.
/// Assign ballPrefab and ballSpawnPoint in the Inspector.
/// </summary>
public class GameDirector : MonoBehaviour
{
    [Header("Ball Spawning")]
    [Tooltip("The Ball prefab to instantiate on respawn.")]
    [SerializeField] private GameObject ballPrefab;

    [Tooltip("Where the ball appears on respawn. Point this at the plunger lane.")]
    [SerializeField] private Transform ballSpawnPoint;

    [Tooltip("Seconds to wait after a drain before spawning the next ball.")]
    [SerializeField] private float respawnDelay = 1.5f;

    [Header("Game Over")]
    [Tooltip("Seconds to wait after the last ball drains before reloading the scene.")]
    [SerializeField] private float gameOverDelay = 3f;

    private void OnEnable()
    {
        BallLifeService.OnBallLost  += HandleBallLost;
        BallLifeService.OnGameOver  += HandleGameOver;
    }

    private void OnDisable()
    {
        BallLifeService.OnBallLost  -= HandleBallLost;
        BallLifeService.OnGameOver  -= HandleGameOver;
    }

    private void HandleBallLost(int remaining)
    {
        Debug.Log($"[GameDirector] Ball lost — {remaining} ball(s) remaining. Respawning in {respawnDelay}s.");
        StartCoroutine(RespawnRoutine());
    }

    private void HandleGameOver()
    {
        Debug.Log($"[GameDirector] GAME OVER — restarting in {gameOverDelay}s.");
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnBall();
    }

    private IEnumerator GameOverRoutine()
    {
        // Use real time so the delay isn't affected if timeScale is changed downstream
        yield return new WaitForSecondsRealtime(gameOverDelay);

        Debug.Log("[GameDirector] Game frozen. Press R to restart.");
        Time.timeScale = 0f;
    }

    /// <summary>Spawns a ball at the configured spawn point. Also callable directly from cheats.</summary>
    public void SpawnBall()
    {
        if (ballPrefab == null)
        {
            Debug.LogError("[GameDirector] ballPrefab is not assigned.");
            return;
        }

        if (ballSpawnPoint == null)
        {
            Debug.LogError("[GameDirector] ballSpawnPoint is not assigned.");
            return;
        }

        Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        Debug.Log("[GameDirector] Ball spawned.");
    }

    /// <summary>Reloads the active scene, resetting all session state.</summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
