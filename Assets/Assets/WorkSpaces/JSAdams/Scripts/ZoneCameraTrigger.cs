
// ----- ZoneCameraTrigger.cs START -----

using UnityEngine;

public class ZoneCameraTrigger : MonoBehaviour
{
    [SerializeField] private Zone zone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball")) return;

        if (ZoneCameraManager.Instance.GetPrimaryBall() != other.transform)
            return;

        ZoneCameraManager.Instance.SetZone(zone);
        Debug.Log("Entered zone: " + zone);
    }
}
// ----- ZoneCameraTrigger.cs END -----