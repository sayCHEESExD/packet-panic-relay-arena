using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FusionTestLauncher : MonoBehaviour
{
    [Header("Session Settings")]
    public string defaultSessionName = "PacketPanicTestRoom";

    private NetworkRunner runner;
    private bool isStarting;

    public bool IsRunning
    {
        get { return runner != null && runner.IsRunning; }
    }

    public async void JoinDefaultRoom()
    {
        await StartSharedSession(defaultSessionName);
    }

    public async void JoinRoom(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName))
        {
            roomName = defaultSessionName;
        }

        await StartSharedSession(roomName);
    }

    private async Task StartSharedSession(string sessionName)
    {
        if (isStarting || IsRunning)
        {
            return;
        }

        isStarting = true;

        Debug.Log("Starting Fusion Shared session: " + sessionName);

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        var sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

        NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(
            SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            LoadSceneMode.Single
        );

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            Scene = sceneInfo,
            SceneManager = sceneManager
        });

        isStarting = false;

        if (result.Ok)
        {
            Debug.Log("Fusion Shared session started successfully.");
        }
        else
        {
            Debug.LogError("Fusion failed to start: " + result.ShutdownReason);
        }
    }
}