
// -----CameraFollow.cs START-----

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;          // the ball
    public float followSpeed = 5f;

    private float minY;               // camera never goes below this

    void Start()
    {
        minY = transform.position.y;
    }

    void LateUpdate()
    {
        if (!target)
            return;

        float targetY = Mathf.Max(minY, target.position.y);

        Vector3 desiredPosition = new Vector3(
            transform.position.x,
            targetY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );
    }
}

// -----CameraFollow.cs END-----