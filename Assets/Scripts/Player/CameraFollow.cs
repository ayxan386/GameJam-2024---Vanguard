using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private float bounceIntensity = 0.1f;
    [SerializeField] private float maxBounce = 0.5f;
    [SerializeField] private Vector3 offset;
    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = transform;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Smoothly follow the player's position
        Vector3 desiredPosition = target.position + offset;
        var position = cameraTransform.position;

        position =
            Vector3.Lerp(position, desiredPosition, smoothSpeed * Time.deltaTime);

        float bounce = Mathf.Sin(Time.time * bounceIntensity) * maxBounce;
        position.y += bounce;

        // Apply effects
        cameraTransform.position = position;
    }
}