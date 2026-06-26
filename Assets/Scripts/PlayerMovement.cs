using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float rotationSpeed = 12f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 moveDirection;
    private Animator animator;

    private bool isRunning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (IsMatchInactive())
        {
            moveInput = Vector3.zero;
            moveDirection = Vector3.zero;
            isRunning = false;
            UpdateAnimation();
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(horizontal, 0f, vertical).normalized;

        isRunning =
            moveInput.sqrMagnitude > 0.01f &&
            (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

        CalculateCameraRelativeMovement();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 movementStep = moveDirection * currentSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = rb.position + movementStep;

        rb.MovePosition(newPosition);

        rb.angularVelocity = Vector3.zero;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            Quaternion smoothedRotation = Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            rb.MoveRotation(smoothedRotation);
        }
    }

    private void CalculateCameraRelativeMovement()
    {
        if (cameraTransform == null)
        {
            moveDirection = moveInput;
            return;
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = cameraForward * moveInput.z + cameraRight * moveInput.x;
        moveDirection.Normalize();
    }

    private void UpdateAnimation()
    {
        if (animator == null)
        {
            return;
        }

        float animationSpeed = 0f;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            animationSpeed = isRunning ? 1f : 0.5f;
        }

        animator.SetFloat("Speed", animationSpeed);
    }
    public float GetAnimationSpeedValue()
{
    if (moveInput.sqrMagnitude <= 0.01f)
    {
        return 0f;
    }

    return isRunning ? 1f : 0.5f;
}
private bool IsMatchInactive()
{
    if (FusionGameState.Instance != null)
    {
        if (!FusionGameState.Instance.HasSpawned)
        {
            return false;
        }

        return !FusionGameState.Instance.IsMatchActive;
    }

    if (MatchManager.Instance != null)
    {
        return !MatchManager.Instance.IsMatchActive;
    }

    return false;
}
}