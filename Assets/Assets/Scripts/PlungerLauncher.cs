using UnityEngine;

public class PlungerControl : MonoBehaviour
{
    public KeyCode launchKey = KeyCode.Space;
    public float maxPullBackDistance = 1.5f;
    public float pullSpeed = 6f;
    public float returnSpeed = 75f;
    public float maxLaunchForce = 1000f;
    public Transform plungerTip;

    private Vector3 startLocalPos;
    private bool isCharging = false;
    private float chargeAmount = 0f;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKey(launchKey))
        {
            isCharging = true;
            chargeAmount += Time.deltaTime * pullSpeed;
            chargeAmount = Mathf.Clamp(chargeAmount, 0f, maxPullBackDistance);

            // Move on local Y-axis instead of Z
            transform.localPosition = startLocalPos - transform.up * chargeAmount;
        }
        else if (isCharging)
        {
            // Apply launch force
            Collider[] hit = Physics.OverlapSphere(plungerTip.position, 0.2f);
            foreach (var obj in hit)
            {
                Rigidbody rb = obj.attachedRigidbody;
                if (rb != null)
                {
                    float launchForce = Mathf.Lerp(0f, maxLaunchForce, chargeAmount / maxPullBackDistance);
                    rb.AddForce(transform.up * launchForce); // Apply along local Y
                }
            }

            // Reset
            StartCoroutine(ResetPosition());
            chargeAmount = 0f;
            isCharging = false;
        }
    }

    private System.Collections.IEnumerator ResetPosition()
    {
        while (Vector3.Distance(transform.localPosition, startLocalPos) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, startLocalPos, returnSpeed * Time.deltaTime);
            yield return null;
        }

        transform.localPosition = startLocalPos;
    }
}
