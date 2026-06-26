using Fusion;
using UnityEngine;

public class FusionSimpleTransformSync : NetworkBehaviour
{
    [Header("Remote Smoothing")]
    public float positionLerpSpeed = 15f;
    public float rotationLerpSpeed = 15f;

    private Animator animator;
    private PlayerMovement movement;

    [Networked]
    private Vector3 NetworkedPosition { get; set; }

    [Networked]
    private Quaternion NetworkedRotation { get; set; }

    [Networked]
    private float NetworkedAnimationSpeed { get; set; }

    public override void Spawned()
    {
        animator = GetComponentInChildren<Animator>();
        movement = GetComponent<PlayerMovement>();

        NetworkedPosition = transform.position;
        NetworkedRotation = transform.rotation;
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            NetworkedPosition = transform.position;
            NetworkedRotation = transform.rotation;

            if (movement != null)
            {
                NetworkedAnimationSpeed = movement.GetAnimationSpeedValue();
            }
        }
    }

    public override void Render()
    {
        if (!Object.HasStateAuthority)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                NetworkedPosition,
                positionLerpSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                NetworkedRotation,
                rotationLerpSpeed * Time.deltaTime
            );
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", NetworkedAnimationSpeed);
        }
    }
}