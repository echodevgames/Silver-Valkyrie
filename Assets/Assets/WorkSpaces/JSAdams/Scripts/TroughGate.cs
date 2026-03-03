
//----- TroughGate.cs START -----
using UnityEngine;
using System.Collections;

public class TroughGate : MonoBehaviour
{
    [SerializeField] private Transform gateVisual;
    [SerializeField] private float openOffsetY = 1f;
    [SerializeField] private float moveSpeed = 20f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool isOpen = false;
    private bool isMoving = false;

    private void Awake()
    {
        closedPosition = gateVisual.localPosition;
        openPosition = closedPosition + Vector3.up * openOffsetY;
    }

    public void ForceOpen()
    {
        if (isOpen || isMoving) return;
        StartCoroutine(MoveGate(openPosition));
    }

    public void ForceClose()
    {
        if (!isOpen || isMoving) return;
        StartCoroutine(MoveGate(closedPosition));
    }

    private IEnumerator MoveGate(Vector3 target)
    {
        isMoving = true;

        while (Vector3.Distance(gateVisual.localPosition, target) > 0.01f)
        {
            gateVisual.localPosition = Vector3.MoveTowards(
                gateVisual.localPosition,
                target,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        gateVisual.localPosition = target;
        isOpen = target == openPosition;
        isMoving = false;
    }
}
//----- TroughGate.cs END -----