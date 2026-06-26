using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Position")]
    public float distance = 7f;
    public float targetHeight = 1.4f;

    [Header("Orbit Settings")]
    public float mouseSensitivity = 4f;
    public float minPitch = 10f;
    public float maxPitch = 55f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 4f;
    public float minDistance = 3.5f;
    public float maxDistance = 11f;

    [Header("Smoothing")]
    public float followSmoothness = 12f;

    private float yaw = 0f;
    private float pitch = 25f;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        HandleCameraInput();
        UpdateCameraPosition();
    }

    private void HandleCameraInput()
    {
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void UpdateCameraPosition()
    {
        Vector3 targetPoint = target.position + Vector3.up * targetHeight;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = targetPoint + rotation * new Vector3(0f, 0f, -distance);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSmoothness * Time.deltaTime
        );

        transform.LookAt(targetPoint);
    }
}