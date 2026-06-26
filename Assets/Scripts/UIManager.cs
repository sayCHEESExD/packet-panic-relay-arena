using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Text")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI statusText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void UpdateTimer(float timeRemaining)
    {
        if (timerText == null)
        {
            return;
        }

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}