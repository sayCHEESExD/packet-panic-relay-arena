using Fusion;
using UnityEngine;

public class FusionPlayerShooter : NetworkBehaviour
{
    [Header("References")]
    public ObjectPooler objectPooler;
    public Transform muzzlePoint;

    [Header("Shooting Settings")]
    public float shootCooldown = 0.35f;

    private float lastShootTime;

    private void Update()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }

       if (IsMatchInactive())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (Time.time < lastShootTime + shootCooldown)
        {
            return;
        }

        if (muzzlePoint == null)
        {
            Debug.LogError("MuzzlePoint is missing on FusionPlayerShooter.");
            return;
        }

        lastShootTime = Time.time;

        Vector3 shootPosition = muzzlePoint.position;
        Vector3 shootDirection = transform.forward;

        RPC_Shoot(shootPosition, shootDirection);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_Shoot(Vector3 shootPosition, Vector3 shootDirection)
    {
        if (objectPooler == null)
        {
            objectPooler = ObjectPooler.Instance;
        }

        if (objectPooler == null)
        {
            Debug.LogError("No ObjectPooler found for projectile spawn.");
            return;
        }

        GameObject projectileObject = objectPooler.GetPooledProjectile();

        if (projectileObject == null)
        {
            return;
        }

        projectileObject.transform.position = shootPosition;
        projectileObject.transform.rotation = Quaternion.LookRotation(shootDirection);
        projectileObject.SetActive(true);

        PulseProjectile projectile = projectileObject.GetComponent<PulseProjectile>();

        if (projectile != null)
        {
            projectile.Launch(shootDirection);
        }
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