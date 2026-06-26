using Fusion;
using UnityEngine;

public class FusionPlayerSetup : NetworkBehaviour
{
    private PlayerMovement movement;
    private FusionPlayerShooter shooter;

    public override void Spawned()
    {
        movement = GetComponent<PlayerMovement>();
        shooter = GetComponent<FusionPlayerShooter>();

        bool isLocalPlayer = Object.HasStateAuthority;

        Debug.Log("FusionPlayerSetup spawned on " + gameObject.name + " | Is local: " + isLocalPlayer);

        if (movement != null)
        {
            movement.enabled = isLocalPlayer;
        }

        if (shooter != null)
        {
            shooter.enabled = true;
        }

        if (isLocalPlayer && Camera.main != null)
        {
            CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();

            if (cameraFollow != null)
            {
                cameraFollow.target = transform;
                Debug.Log("Camera now following: " + gameObject.name);
            }
        }
    }
}