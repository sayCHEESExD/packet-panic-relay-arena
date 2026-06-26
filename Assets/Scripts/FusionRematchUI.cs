using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FusionRematchUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject rematchPanel;

    [Header("UI")]
    public TextMeshProUGUI finalText;
    public Button playAgainButton;

    private bool lastMatchEndedState = false;

    private void Start()
    {
        if (rematchPanel != null)
        {
            rematchPanel.SetActive(false);
        }

        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        }
    }

    private void Update()
    {
        if (FusionGameState.Instance == null || !FusionGameState.Instance.HasSpawned)
        {
            return;
        }

        bool matchEnded = FusionGameState.Instance.IsMatchEnded;

        if (matchEnded != lastMatchEndedState)
        {
            lastMatchEndedState = matchEnded;

            if (matchEnded)
            {
                ShowRematchPanel();
            }
            else
            {
                HideRematchPanel();
            }
        }
    }

    private void ShowRematchPanel()
    {
        if (rematchPanel != null)
        {
            rematchPanel.SetActive(true);
        }

        if (finalText != null && FusionGameState.Instance != null)
        {
            finalText.text = "MATCH OVER\nFinal Score: " + FusionGameState.Instance.SafeScore;
        }

        if (playAgainButton != null)
        {
            playAgainButton.interactable = true;
        }
    }

    private void HideRematchPanel()
    {
        if (rematchPanel != null)
        {
            rematchPanel.SetActive(false);
        }

        if (playAgainButton != null)
        {
            playAgainButton.interactable = true;
        }
    }

    private void OnPlayAgainClicked()
    {
        if (FusionGameState.Instance == null || !FusionGameState.Instance.HasSpawned)
        {
            return;
        }

        if (playAgainButton != null)
        {
            playAgainButton.interactable = false;
        }

        FusionGameState.Instance.RequestRestartMatch();
    }
}