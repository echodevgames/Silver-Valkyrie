using UnityEngine;
using Unity.Cinemachine;

public class ZoneCameraManager : MonoBehaviour
{
    public static ZoneCameraManager Instance { get; private set; }

    [SerializeField] private CinemachineCamera lower;
    [SerializeField] private CinemachineCamera middle;
    [SerializeField] private CinemachineCamera upper;
    private Zone currentZone;
    private bool hasInitialized = false;
    private Transform primaryBall;


    private void Awake()
    {
        Instance = this;
    }

    public void SetZone(Zone zone)
    {
        if (!hasInitialized)
        {
            hasInitialized = true;
            currentZone = zone;

            lower.Priority = zone == Zone.Lower ? 100 : 10;
            middle.Priority = zone == Zone.Middle ? 100 : 10;
            upper.Priority = zone == Zone.Upper ? 100 : 10;

            return; // 🚫 do NOT trigger slow on first set
        }

        if (zone == currentZone) return;

        currentZone = zone;

        lower.Priority = zone == Zone.Lower ? 100 : 10;
        middle.Priority = zone == Zone.Middle ? 100 : 10;
        upper.Priority = zone == Zone.Upper ? 100 : 10;

        TransitionTimeController.Instance?.TriggerZoneSlow();
    }
    public void SetPrimaryBall(Transform ball)
    {
        primaryBall = ball;

        lower.Follow = ball;
        middle.Follow = ball;
        upper.Follow = ball;
    }
    public Transform GetPrimaryBall()
    {
        return primaryBall;
    }
}

public enum Zone
{
    Lower,
    Middle,
    Upper
}