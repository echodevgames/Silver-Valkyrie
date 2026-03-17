using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Developer overlay that lists all active cheat keys and their effects.
/// Toggle visibility with H. Compiled out of non-development builds automatically.
/// </summary>
public class CheatSheetUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI contentText;

    private const string CheatContent =
        "<color=#E8C840><b>CHEAT KEYS</b></color>\n" +
        "<color=#9AAAB8>──────────────────</color>\n\n" +
        "<color=#5AB8D8><b>[B]</b></color>  <color=#ECE8D8>Spawn ball</color>\n" +
        "<color=#5AB8D8><b>[K]</b></color>  <color=#ECE8D8>Kill ball</color>  <color=#889098>(costs a life)</color>\n" +
        "<color=#5AB8D8><b>[L]</b></color>  <color=#ECE8D8>+1 life</color>\n" +
        "<color=#5AB8D8><b>[R]</b></color>  <color=#ECE8D8>Restart</color>  <color=#889098>(unfreezes game over)</color>\n\n" +
        "<color=#889098><size=80%>[H] to close</size></color>";

    private void Awake()
    {
        if (contentText != null)
            contentText.text = CheatContent;

        if (panel != null)
            panel.SetActive(false);
    }

    private void Update()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        return;
#endif
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.hKey.wasPressedThisFrame)
            Toggle();
    }

    /// <summary>Flips the cheat sheet panel between visible and hidden.</summary>
    public void Toggle()
    {
        if (panel != null)
            panel.SetActive(!panel.activeSelf);
    }
}
