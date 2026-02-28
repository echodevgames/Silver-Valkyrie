
//----- TroughGate.cs START -----

using UnityEngine;
using System.Collections;

public class TroughGate : MonoBehaviour
{
    [SerializeField] private Transform gateVisual;
    [Header("Movement")]
    [SerializeField] private float openOffsetY = 1f;
    [SerializeField] private float moveSpeed = 8f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool isOpen = false;
    private bool isMoving = false;

    private void Awake()
    {
        closedPosition = gateVisual.localPosition;
        openPosition = closedPosition + Vector3.up * openOffsetY;
    }
    private void Start()
    {
        OpenGate();
    }

    public void OpenGate()
    {
        if (isOpen || isMoving) return;
        StartCoroutine(MoveGate(openPosition));
    }

    public void CloseGate()
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
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Trigger exit detected");

        if (!isOpen) return;

        if (other.CompareTag("Ball"))
        {
            Debug.Log("Ball exited trigger — closing gate");
            CloseGate();
        }
    }
}
//----- TroughGate.cs END -----