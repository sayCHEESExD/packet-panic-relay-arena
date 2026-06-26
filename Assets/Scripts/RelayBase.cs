using UnityEngine;

public class RelayBase : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerCarry playerCarry = other.GetComponent<PlayerCarry>();

        if (playerCarry == null)
        {
            return;
        }

        playerCarry.DeliverPacket();
    }
}