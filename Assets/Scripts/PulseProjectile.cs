using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PulseProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 16f;
    public float lifetime = 1.5f;

    private Rigidbody rb;
    private float lifeTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnEnable()
    {
        lifeTimer = 0f;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime)
        {
            DeactivateProjectile();
        }
    }

    public void Launch(Vector3 direction)
    {
        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z).normalized;

        if (flatDirection.sqrMagnitude < 0.01f)
        {
            flatDirection = Vector3.forward;
        }

        transform.rotation = Quaternion.LookRotation(flatDirection);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = flatDirection * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DataPacket") || other.CompareTag("RelayBase"))
        {
            return;
        }

        PlayerHitHandler hitHandler = other.GetComponent<PlayerHitHandler>();

        if (hitHandler != null)
        {
            Vector3 hitDirection = other.transform.position - transform.position;
            hitHandler.ReceiveHit(hitDirection);
        }

        DeactivateProjectile();
    }

    private void DeactivateProjectile()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}