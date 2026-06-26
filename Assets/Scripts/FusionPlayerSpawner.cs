using Fusion;
using UnityEngine;

public class FusionPlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [Header("Player Prefab")]
    public NetworkObject playerPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("PlayerJoined callback: " + player);

        if (player != Runner.LocalPlayer)
        {
            return;
        }

        SpawnLocalPlayer(player);
    }

    private void SpawnLocalPlayer(PlayerRef player)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("FusionPlayerSpawner is missing NetworkPlayerBot prefab.");
            return;
        }

        Vector3 spawnPosition = GetSpawnPosition();
        Quaternion spawnRotation = Quaternion.identity;

        NetworkObject playerObject = Runner.Spawn(
            playerPrefab,
            spawnPosition,
            spawnRotation,
            player
        );

        if (playerObject == null)
        {
            Debug.LogError("Runner.Spawn returned null. Check NetworkPlayerBot prefab and Fusion prefab table.");
            return;
        }

        playerObject.name = "NetworkPlayerBot_" + player.PlayerId;

        Runner.SetPlayerObject(player, playerObject);

        Debug.Log("Spawned player object: " + playerObject.name + " at " + playerObject.transform.position);
        Debug.Log("Runner PlayerObject after set: " + Runner.GetPlayerObject(player));
    }

    private Vector3 GetSpawnPosition()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4f, 4f));
        }

        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index].position;
    }
}