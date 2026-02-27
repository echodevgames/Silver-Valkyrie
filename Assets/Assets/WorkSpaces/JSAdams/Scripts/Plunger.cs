// ----- Plunger.cs START -----
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Plunger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D ramRb;
    [SerializeField] private Transform housing;
    [SerializeField] private InputActionReference launchAction;

    [Header("Tuning")]
    [SerializeField] private float maxPullDistance = 1.2f;
    [SerializeField] private float pullDuration = 0.6f;

    [Tooltip("How fast the slam happens")]
    [SerializeField] private float slamSpeed = 30f;

    [Tooltip("Visual overshoot amount")]
    [SerializeField] private float overshootAmount = 0.2f;

    [Tooltip("How fast it settles after slam")]
    [SerializeField] private float settleSpeed = 10f;

    private Vector2 startPos;

    private bool isPulling;
    private bool isSlamming;
    private bool isSettling;

    private float pullTimer;
    private float currentOffset;
    private float settleTimer;
    private void Awake()
    {
        startPos = ramRb.position;
    }

    private void OnEnable()
    {
        launchAction.action.Enable();
        launchAction.action.performed += ctx => StartPull();
        launchAction.action.canceled += ctx => Release();
    }

    private void OnDisable()
    {
        launchAction.action.Disable();
    }

    private void StartPull()
    {
        isPulling = true;
        isSlamming = false;
        isSettling = false;
        pullTimer = 0f;
    }

    private void Release()
    {
        isPulling = false;
        isSlamming = true;

        // CRITICAL: give real velocity for physical impact
        ramRb.linearVelocity = Vector2.up * slamSpeed;

        StartCoroutine(ShakeHousing());
    }

    private void FixedUpdate()
    {
        if (isPulling)
        {
            pullTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(pullTimer / pullDuration);

            float eased = 1f - Mathf.Pow(1f - t, 2f);
            currentOffset = eased * maxPullDistance;

            MoveToOffset(currentOffset);
        }
        else if (isSlamming)
        {
            // wait until ram reaches neutral
            if (ramRb.position.y >= startPos.y)
            {
                isSlamming = false;
                isSettling = true;
                settleTimer = 0f;


            }
        }
        else if (isSettling)
        {
            settleTimer += Time.fixedDeltaTime;

            float damping = 4f;     // higher = stops faster
            float frequency = 12f;  // higher = tighter swings

            float offset =
                overshootAmount *
                Mathf.Exp(-damping * settleTimer) *
                Mathf.Cos(frequency * settleTimer);

            MoveToOffset(offset);

            if (Mathf.Abs(offset) < 0.01f)
            {
                isSettling = false;
                MoveToOffset(0f);
            }
        }
        else
        {
            MoveToOffset(0f);
        }
    }

    private void MoveToOffset(float offset)
    {
        Vector2 target = new Vector2(
            startPos.x,
            startPos.y - offset
        );

        ramRb.MovePosition(target);
    }

    private IEnumerator ShakeHousing()
    {
        float duration = 0.12f;
        float strength = 0.06f;

        Vector3 original = housing.localPosition;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            housing.localPosition =
                original + (Vector3)Random.insideUnitCircle * strength;

            yield return null;
        }

        housing.localPosition = original;
    }
}
// ----- Plunger.cs END -----