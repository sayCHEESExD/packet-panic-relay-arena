using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    public ObjectPooler objectPooler;
    public Transform muzzlePoint;

    [Header("Shooting Settings")]
    public float shootCooldown = 0.35f;

    private float lastShootTime;

    private void Update()
    {
        if (MatchManager.Instance != null && !MatchManager.Instance.IsMatchActive)
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

        if (objectPooler == null)
        {
            objectPooler = ObjectPooler.Instance;
        }

        if (objectPooler == null)
        {
            Debug.LogError("No ObjectPooler assigned to PlayerShooter.");
            return;
        }

        if (muzzlePoint == null)
        {
            Debug.LogError("MuzzlePoint is missing on PlayerShooter.");
            return;
        }

        GameObject projectileObject = objectPooler.GetPooledProjectile();

        if (projectileObject == null)
        {
            return;
        }

        lastShootTime = Time.time;

        projectileObject.transform.position = muzzlePoint.position;
        projectileObject.transform.rotation = muzzlePoint.rotation;
        projectileObject.SetActive(true);

        PulseProjectile projectile = projectileObject.GetComponent<PulseProjectile>();

        if (projectile == null)
        {
            Debug.LogError("PulseProjectile prefab is missing the PulseProjectile script.");
            return;
        }

        projectile.Launch(transform.forward);
    }
}