
// ----- TransitionTimeController.cs START -----

using UnityEngine;
using System.Collections;

public class TransitionTimeController : MonoBehaviour
{
    public static TransitionTimeController Instance;

    [Header("Zone Transition Slow")]
    [SerializeField] private float slowAmount = 0.65f;
    [SerializeField] private float easeDuration = 0.15f;
    [SerializeField] private float holdDuration = 0.1f;

    private Coroutine transitionRoutine;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerZoneSlow()
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(ZoneSlowRoutine());
    }

    private IEnumerator ZoneSlowRoutine()
    {
        float downTime = 0.25f;
        float upTime = 0.4f;

        // Ease down
        yield return LerpTimeScale(1f, slowAmount, downTime);

        // Hold near freeze
        yield return new WaitForSecondsRealtime(holdDuration);

        // Ease back up slower
        yield return LerpTimeScale(slowAmount, 1f, upTime);
    }

    private IEnumerator LerpTimeScale(float from, float to, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;
            t = Mathf.SmoothStep(0f, 1f, t);
            Time.timeScale = Mathf.Lerp(from, to, t);
            yield return null;
        }

        Time.timeScale = to;
    }
}

// ----- TransitionTimeController.cs END -----