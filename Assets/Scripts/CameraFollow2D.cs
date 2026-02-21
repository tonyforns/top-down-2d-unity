using UnityEngine;

[DisallowMultipleComponent]
public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private float smoothTime = 0.12f;
    [SerializeField] private bool useUnscaledTime = false;

    [Header("Clamp (optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds = new Vector2(-100, -100);
    [SerializeField] private Vector2 maxBounds = new Vector2(100, 100);

    [Header("Pixel Snap (optional)")]
    [SerializeField] private bool pixelSnap = false;
    [SerializeField] private float pixelsPerUnit = 16f;

    private Vector3 velocity;

    private void Reset()
    {
        var cam = GetComponent<Camera>();
        if (cam != null) cam.orthographic = true;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        if (useBounds)
        {
            desired.x = Mathf.Clamp(desired.x, minBounds.x, maxBounds.x);
            desired.y = Mathf.Clamp(desired.y, minBounds.y, maxBounds.y);
        }

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        Vector3 smoothed = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref velocity,
            smoothTime,
            Mathf.Infinity,
            dt
        );

        if (pixelSnap && pixelsPerUnit > 0f)
        {
            smoothed.x = Mathf.Round(smoothed.x * pixelsPerUnit) / pixelsPerUnit;
            smoothed.y = Mathf.Round(smoothed.y * pixelsPerUnit) / pixelsPerUnit;
        }

        transform.position = smoothed;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        velocity = Vector3.zero;
        if (target != null)
        {
            transform.position = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                transform.position.z
            );
        }
    }
}