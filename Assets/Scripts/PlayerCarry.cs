using Fusion;
using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    [Header("Carry Settings")]
    public Transform carryPoint;

    private DataPacket carriedPacket;

    public bool HasPacket()
    {
        return carriedPacket != null;
    }

    public void TryPickUpPacket(DataPacket packet)
    {
        if (carriedPacket != null)
        {
            return;
        }

        if (FusionGameState.Instance != null)
        {
            int playerId = GetLocalPlayerId();

            if (playerId != -1)
            {
                FusionGameState.Instance.RequestPickup(playerId);
            }

            return;
        }

        SetCarriedPacket(packet);
    }

    public void SetCarriedPacket(DataPacket packet)
    {
        if (packet == null || carryPoint == null)
        {
            return;
        }

        carriedPacket = packet;
        carriedPacket.AttachToPlayer(carryPoint);
    }

    public void ClearCarriedPacket()
    {
        carriedPacket = null;
    }

    public void DeliverPacket()
    {
        if (FusionGameState.Instance != null)
        {
            int playerId = GetLocalPlayerId();

            if (playerId != -1)
            {
                FusionGameState.Instance.RequestDelivery(playerId);
            }

            return;
        }

        if (carriedPacket == null)
        {
            return;
        }

        DataPacket deliveredPacket = carriedPacket;
        carriedPacket = null;

        deliveredPacket.Respawn();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(1);
        }
    }

    public void DropPacket()
    {
        if (carriedPacket == null)
        {
            return;
        }

        DataPacket droppedPacket = carriedPacket;
        carriedPacket = null;

        Vector3 dropPosition = transform.position + transform.forward * 1.2f;
        dropPosition.y = 0.5f;

        droppedPacket.DropAt(dropPosition);
    }

    private int GetLocalPlayerId()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            return -1;
        }

        if (!networkObject.HasStateAuthority)
        {
            return -1;
        }

        return networkObject.StateAuthority.PlayerId;
    }
}