using UnityEngine;

public class FlipperCollisionHandler : MonoBehaviour
{
    [HideInInspector]
    public FlipperPivotControl parentFlipper;

    private void OnTriggerEnter(Collider other)
    {
        if (parentFlipper != null)
        {
            parentFlipper.OnChildTriggerEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (parentFlipper != null)
        {
            parentFlipper.OnChildTriggerExit(other);
        }
    }
}