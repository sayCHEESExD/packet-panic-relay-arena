using UnityEngine;

public class PacketSpawner : MonoBehaviour
{
    public static PacketSpawner Instance;

    [Header("Packet Reference")]
    public DataPacket dataPacket;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    public float packetHeight = 0.5f;

    private int nextSpawnIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MovePacketToSpawn(0);
        nextSpawnIndex = 1;
    }

    public void RespawnPacket()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("PacketSpawner has no spawn points assigned.");
            return;
        }

        MovePacketToSpawn(nextSpawnIndex);

        nextSpawnIndex++;
        if (nextSpawnIndex >= spawnPoints.Length)
        {
            nextSpawnIndex = 0;
        }
    }

    private void MovePacketToSpawn(int index)
    {
        if (dataPacket == null)
        {
            Debug.LogError("PacketSpawner is missing DataPacket reference.");
            return;
        }

        Vector3 spawnPosition = spawnPoints[index].position;
        spawnPosition.y = packetHeight;

        dataPacket.MoveToSpawnPoint(spawnPosition);

        Debug.Log("Packet moved to spawn index: " + index);
    }
}