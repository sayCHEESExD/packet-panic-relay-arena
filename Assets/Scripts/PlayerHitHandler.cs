using System.Collections;
using Fusion;
using UnityEngine;

public class PlayerHitHandler : MonoBehaviour
{
    [Header("Hit Settings")]
    public float stunDuration = 0.5f;
    public float knockbackForce = 5f;
    public float packetDropDistance = 1.8f;

    [Header("Visual Feedback")]
    public Renderer bodyRenderer;
    public Color normalColor = Color.white;
    public Color hitColor = Color.red;

    private Rigidbody rb;
    private PlayerMovement movement;
    private PlayerCarry playerCarry;
    private NetworkObject networkObject;
    private bool isStunned;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
        playerCarry = GetComponent<PlayerCarry>();
        networkObject = GetComponent<NetworkObject>();
    }

    public void ReceiveHit(Vector3 hitDirection)
    {
        if (isStunned)
        {
            return;
        }

        TryDropPacketNearPlayer(hitDirection);

        if (rb != null)
        {
            Vector3 flatDirection = new Vector3(hitDirection.x, 0f, hitDirection.z).normalized;

            if (flatDirection.sqrMagnitude > 0.01f)
            {
                rb.AddForce(flatDirection * knockbackForce, ForceMode.Impulse);
            }
        }

        StartCoroutine(StunRoutine());
    }

    private void TryDropPacketNearPlayer(Vector3 hitDirection)
    {
        if (FusionGameState.Instance != null && networkObject != null)
        {
            int playerId = networkObject.StateAuthority.PlayerId;

            Vector3 flatDirection = new Vector3(hitDirection.x, 0f, hitDirection.z).normalized;

            if (flatDirection.sqrMagnitude < 0.01f)
            {
                flatDirection = transform.forward;
            }

            Vector3 dropPosition = transform.position - flatDirection * packetDropDistance;
            dropPosition.y = 0.5f;

            FusionGameState.Instance.RequestCarrierDrop(playerId, dropPosition);
            return;
        }

        // Local fallback if Fusion is not active.
        if (playerCarry != null && playerCarry.HasPacket())
        {
            playerCarry.DropPacket();
        }
    }

    private IEnumerator StunRoutine()
    {
        isStunned = true;

        if (movement != null)
        {
            movement.enabled = false;
        }

        if (bodyRenderer != null)
        {
            bodyRenderer.material.color = hitColor;
        }

        yield return new WaitForSeconds(stunDuration);

        if (bodyRenderer != null)
        {
            bodyRenderer.material.color = normalColor;
        }

        if (movement != null)
        {
            movement.enabled = true;
        }

        isStunned = false;
    }
}