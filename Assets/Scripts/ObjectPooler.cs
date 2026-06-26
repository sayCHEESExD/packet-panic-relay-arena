using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [Header("Projectile Pool")]
    public GameObject projectilePrefab;
    public int poolSize = 25;

    private readonly List<GameObject> pooledProjectiles = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate ObjectPooler found on " + gameObject.name + ". This component will be disabled.");
            enabled = false;
            return;
        }

        Instance = this;

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab is missing on the scene ObjectPooler.");
            return;
        }

        CreatePool();
    }

    private void CreatePool()
    {
        pooledProjectiles.Clear();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform);
            projectile.name = "Pooled_PulseProjectile_" + i;
            projectile.SetActive(false);
            pooledProjectiles.Add(projectile);
        }

        Debug.Log("Projectile pool created with " + poolSize + " projectiles.");
    }

    public GameObject GetPooledProjectile()
    {
        for (int i = 0; i < pooledProjectiles.Count; i++)
        {
            if (!pooledProjectiles[i].activeInHierarchy)
            {
                return pooledProjectiles[i];
            }
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("Cannot expand pool because projectilePrefab is missing.");
            return null;
        }

        GameObject extraProjectile = Instantiate(projectilePrefab, transform);
        extraProjectile.name = "Pooled_PulseProjectile_Extra";
        extraProjectile.SetActive(false);
        pooledProjectiles.Add(extraProjectile);

        return extraProjectile;
    }
}