// ----- StartUIController.cs START -----

using UnityEngine;
using TMPro;
using System.Collections;

public class StartUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;
    [SerializeField] private TextMeshProUGUI countdownText;

    [SerializeField] private float blinkSpeed = 0.6f;
    [SerializeField] private float countdownStepDuration = 0.75f;

    private Coroutine blinkRoutine;

    private void OnEnable()
    {
        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    public void StopBlinking()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        pressAnyKeyText.gameObject.SetActive(false);
    }

    public IEnumerator PlayCountdown()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(countdownStepDuration);
        }

        countdownText.gameObject.SetActive(false);
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            pressAnyKeyText.enabled = !pressAnyKeyText.enabled;
            yield return new WaitForSecondsRealtime(blinkSpeed);
        }
    }
}

// ----- StartUIController.cs END -----
