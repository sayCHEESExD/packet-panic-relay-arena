using Fusion;
using UnityEngine;

public class FusionGameState : NetworkBehaviour
{
    public static FusionGameState Instance;

    [Header("Match Settings")]
    public float matchDuration = 120f;
    public float countdownDuration = 3f;

    [Header("Packet")]
    public DataPacket dataPacket;
    public Transform[] packetSpawnPoints;
    public float packetHeight = 0.5f;

    [Header("Pickup Lock")]
    public float samePlayerRepickDelay = 1.0f;

    [Networked] public float CountdownRemaining { get; set; }
    [Networked] public int MatchPhase { get; set; }

    [Networked] public int Score { get; set; }
    [Networked] public float TimeRemaining { get; set; }
    [Networked] public NetworkBool MatchActive { get; set; }

    [Networked] public int PacketSpawnIndex { get; set; }
    [Networked] public int PacketCarrierPlayerId { get; set; }
    [Networked] public NetworkBool PacketDropped { get; set; }
    [Networked] public Vector3 PacketDropPosition { get; set; }

    [Networked] public int LastDroppedPlayerId { get; set; }
    [Networked] public float PacketPickupUnlockTime { get; set; }

    [Networked] private NetworkBool Initialized { get; set; }
    [Networked] private int PacketVersion { get; set; }

    private int lastAppliedPacketVersion = -1;
private bool hasSpawned;

public bool HasSpawned
{
    get { return hasSpawned; }
}

public bool IsMatchActive
{
    get
    {
        if (!hasSpawned)
        {
            return false;
        }

        return MatchPhase == 1;
    }
}

public bool IsMatchEnded
{
    get
    {
        if (!hasSpawned)
        {
            return false;
        }

        return MatchPhase == 2;
    }
}

public bool IsCountingDown
{
    get
    {
        if (!hasSpawned)
        {
            return false;
        }

        return MatchPhase == 0;
    }
}

public int SafeScore
{
    get
    {
        if (!hasSpawned)
        {
            return 0;
        }

        return Score;
    }
}

public float SafeTimeRemaining
{
    get
    {
        if (!hasSpawned)
        {
            return matchDuration;
        }

        return TimeRemaining;
    }
}

    public override void Spawned()
    {
        hasSpawned = true;
        Instance = this;

        if (Object.HasStateAuthority && !Initialized)
        {
            RestartMatchState();
            Initialized = true;

            Debug.Log("FusionGameState initialized by StateAuthority.");
        }
    }

    public override void FixedUpdateNetwork()
{
    if (!Object.HasStateAuthority)
    {
        return;
    }

    if (MatchPhase == 0)
    {
        CountdownRemaining -= Runner.DeltaTime;

        if (CountdownRemaining <= 0f)
        {
            CountdownRemaining = 0f;
            MatchPhase = 1;
            MatchActive = true;
        }

        return;
    }

    if (MatchPhase != 1)
    {
        return;
    }

    TimeRemaining -= Runner.DeltaTime;

    if (TimeRemaining <= 0f)
    {
        TimeRemaining = 0f;
        MatchActive = false;
        MatchPhase = 2;
    }
}

    public override void Render()
    {
        UpdateUI();
        ApplyPacketVisualState();
    }

    public void RequestRestartMatch()
    {
        if (Object.HasStateAuthority)
        {
            RestartMatchState();
        }
        else
        {
            RPC_RequestRestartMatch();
        }
    }

    public void RequestPickup(int playerId)
    {
        if (Object.HasStateAuthority)
        {
            HandlePickup(playerId);
        }
        else
        {
            RPC_RequestPickup(playerId);
        }
    }

    public void RequestDelivery(int playerId)
    {
        if (Object.HasStateAuthority)
        {
            HandleDelivery(playerId);
        }
        else
        {
            RPC_RequestDelivery(playerId);
        }
    }

    public void RequestCarrierDrop(int playerId, Vector3 dropPosition)
    {
        if (Object.HasStateAuthority)
        {
            HandleCarrierDrop(playerId, dropPosition);
        }
        else
        {
            RPC_RequestCarrierDrop(playerId, dropPosition);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestRestartMatch()
    {
        RestartMatchState();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestPickup(int playerId)
    {
        HandlePickup(playerId);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestDelivery(int playerId)
    {
        HandleDelivery(playerId);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestCarrierDrop(int playerId, Vector3 dropPosition)
    {
        HandleCarrierDrop(playerId, dropPosition);
    }

    private void RestartMatchState()
    {
        Score = 0;
        TimeRemaining = matchDuration;
        MatchActive = false;
        MatchPhase = 0;
        CountdownRemaining = countdownDuration;

        PacketSpawnIndex = 0;
        PacketCarrierPlayerId = 0;
        PacketDropped = false;
        PacketDropPosition = Vector3.zero;

        LastDroppedPlayerId = 0;
        PacketPickupUnlockTime = 0f;

        PacketVersion++;

        Debug.Log("Match restarted.");
    }

    private void HandlePickup(int playerId)
    {
        if (!MatchActive)
        {
            return;
        }

        if (PacketCarrierPlayerId != 0)
        {
            return;
        }

        if (PacketDropped && playerId == LastDroppedPlayerId)
        {
            if (Runner != null && Runner.SimulationTime < PacketPickupUnlockTime)
            {
                return;
            }
        }

        PacketCarrierPlayerId = playerId;
        PacketDropped = false;
        PacketVersion++;

        Debug.Log("Packet picked up by PlayerId: " + playerId);
    }

    private void HandleDelivery(int playerId)
    {
        if (!MatchActive)
        {
            return;
        }

        if (PacketCarrierPlayerId != playerId)
        {
            return;
        }

        Score += 1;

        PacketCarrierPlayerId = 0;
        PacketDropped = false;

        LastDroppedPlayerId = 0;
        PacketPickupUnlockTime = 0f;

        AdvancePacketSpawn();
        PacketVersion++;

        Debug.Log("Packet delivered by PlayerId: " + playerId + " | Score: " + Score);
    }

    private void HandleCarrierDrop(int playerId, Vector3 dropPosition)
    {
        if (!MatchActive)
        {
            return;
        }

        if (PacketCarrierPlayerId != playerId)
        {
            return;
        }

        dropPosition.y = packetHeight;

        PacketCarrierPlayerId = 0;
        PacketDropped = true;
        PacketDropPosition = dropPosition;

        LastDroppedPlayerId = playerId;

        if (Runner != null)
        {
            PacketPickupUnlockTime = (float)Runner.SimulationTime + samePlayerRepickDelay;
        }

        PacketVersion++;

        Debug.Log("Packet dropped by PlayerId: " + playerId + " at " + dropPosition);
    }

    private void AdvancePacketSpawn()
    {
        if (packetSpawnPoints == null || packetSpawnPoints.Length == 0)
        {
            PacketSpawnIndex = 0;
            return;
        }

        PacketSpawnIndex++;

        if (PacketSpawnIndex >= packetSpawnPoints.Length)
        {
            PacketSpawnIndex = 0;
        }
    }

    private void UpdateUI()
{
    if (!hasSpawned)
    {
        return;
    }

    if (UIManager.Instance == null)
    {
        return;
    }

        UIManager.Instance.UpdateScore(Score);
UIManager.Instance.UpdateTimer(TimeRemaining);

if (MatchPhase == 0)
{
    int countdownNumber = Mathf.CeilToInt(CountdownRemaining);

    if (countdownNumber <= 0)
    {
        UIManager.Instance.ShowStatus("GO!");
    }
    else
    {
        UIManager.Instance.ShowStatus(countdownNumber.ToString());
    }

    return;
}

if (MatchPhase == 1)
{
    if (PacketCarrierPlayerId != 0)
    {
        UIManager.Instance.ShowStatus("Packet is being carried!");
    }
    else if (PacketDropped)
    {
        UIManager.Instance.ShowStatus("Packet dropped! Grab it!");
    }
    else
    {
        UIManager.Instance.ShowStatus("Collect the packet and deliver it!");
    }

    return;
}

if (MatchPhase == 2)
{
    UIManager.Instance.ShowStatus("Match Over! Final Score: " + Score);
}
    }

    private void ApplyPacketVisualState()
{
    if (!hasSpawned)
    {
        return;
    }

    if (dataPacket == null)
    {
        return;
    }

        if (PacketCarrierPlayerId != 0)
        {
            PlayerCarry carrier = FindCarrierByPlayerId(PacketCarrierPlayerId);

            if (carrier == null)
            {
                return;
            }

            ClearAllPlayerCarries();
            carrier.SetCarriedPacket(dataPacket);

            lastAppliedPacketVersion = PacketVersion;
            return;
        }

        ClearAllPlayerCarries();

        if (PacketDropped)
        {
            dataPacket.MoveToSpawnPoint(PacketDropPosition);
        }
        else
        {
            MovePacketToCurrentSpawn();
        }

        lastAppliedPacketVersion = PacketVersion;
    }

    private void MovePacketToCurrentSpawn()
    {
        if (packetSpawnPoints == null || packetSpawnPoints.Length == 0)
        {
            return;
        }

        int safeIndex = Mathf.Clamp(PacketSpawnIndex, 0, packetSpawnPoints.Length - 1);

        Vector3 spawnPosition = packetSpawnPoints[safeIndex].position;
        spawnPosition.y = packetHeight;

        dataPacket.MoveToSpawnPoint(spawnPosition);
    }

    private PlayerCarry FindCarrierByPlayerId(int playerId)
    {
        if (Runner == null)
        {
            return null;
        }

        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            if (player.PlayerId != playerId)
            {
                continue;
            }

            NetworkObject playerObject = Runner.GetPlayerObject(player);

            if (playerObject == null)
            {
                return null;
            }

            return playerObject.GetComponent<PlayerCarry>();
        }

        return null;
    }

    private void ClearAllPlayerCarries()
    {
        if (Runner == null)
        {
            return;
        }

        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            NetworkObject playerObject = Runner.GetPlayerObject(player);

            if (playerObject == null)
            {
                continue;
            }

            PlayerCarry carry = playerObject.GetComponent<PlayerCarry>();

            if (carry != null)
            {
                carry.ClearCarriedPacket();
            }
        }
    }
}