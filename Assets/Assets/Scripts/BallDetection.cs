using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAreaTrigger : MonoBehaviour
{
    private int ballsInArea = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ballsInArea++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ballsInArea--;
            if (ballsInArea <= 0)
            {
                RestartLevel();
            }
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
