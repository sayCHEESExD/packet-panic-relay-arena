using UnityEngine;

public class DataPacket : MonoBehaviour
{
    private Collider packetCollider;
    private Rigidbody rb;

    private void Awake()
    {
        packetCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerCarry playerCarry = other.GetComponent<PlayerCarry>();

        if (playerCarry == null || playerCarry.HasPacket())
        {
            return;
        }

        playerCarry.TryPickUpPacket(this);
    }

    public void AttachToPlayer(Transform carryPoint)
    {
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (packetCollider != null)
        {
            packetCollider.enabled = false;
        }

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void DropAt(Vector3 dropPosition)
    {
        transform.SetParent(null);
        transform.position = dropPosition;
        transform.rotation = Quaternion.identity;

        EnablePacketPhysicsState();
    }

    public void Respawn()
    {
        transform.SetParent(null);

        if (PacketSpawner.Instance != null)
        {
            PacketSpawner.Instance.RespawnPacket();
        }
        else
        {
            EnablePacketPhysicsState();
        }
    }

    public void MoveToSpawnPoint(Vector3 spawnPosition)
    {
        transform.SetParent(null);
        transform.position = spawnPosition;
        transform.rotation = Quaternion.identity;

        EnablePacketPhysicsState();
    }

    private void EnablePacketPhysicsState()
    {
        if (packetCollider != null)
        {
            packetCollider.enabled = true;
        }

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}