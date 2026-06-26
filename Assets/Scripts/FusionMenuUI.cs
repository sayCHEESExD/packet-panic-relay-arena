using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FusionMenuUI : MonoBehaviour
{
    [Header("References")]
    public FusionTestLauncher launcher;

    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject matchHudPanel;

    [Header("Menu UI")]
    public TMP_InputField roomInputField;
    public Button joinButton;
    public TextMeshProUGUI statusText;

    private void Start()
    {
        if (matchHudPanel != null)
        {
            matchHudPanel.SetActive(false);
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }

        if (joinButton != null)
        {
            joinButton.onClick.AddListener(OnJoinClicked);
        }

        if (statusText != null)
        {
            statusText.text = "Enter a room name or use the default.";
        }
    }

    private void OnJoinClicked()
    {
        if (launcher == null)
        {
            Debug.LogError("FusionMenuUI is missing FusionTestLauncher reference.");
            return;
        }

        string roomName = "";

        if (roomInputField != null)
        {
            roomName = roomInputField.text;
        }

        if (string.IsNullOrWhiteSpace(roomName))
        {
            roomName = launcher.defaultSessionName;
        }

        if (statusText != null)
        {
            statusText.text = "Joining room: " + roomName;
        }

        if (joinButton != null)
        {
            joinButton.interactable = false;
        }

        launcher.JoinRoom(roomName);

        Invoke(nameof(ShowMatchHud), 1.0f);
    }

    private void ShowMatchHud()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        if (matchHudPanel != null)
        {
            matchHudPanel.SetActive(true);
        }
    }
}