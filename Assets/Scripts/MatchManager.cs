using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance;

    [Header("Match Settings")]
    public float matchDuration = 120f;

    private float timeRemaining;
    private bool matchActive;

    public bool IsMatchActive
    {
        get { return matchActive; }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartMatch();
    }

    private void Update()
    {
        if (!matchActive)
        {
            return;
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining < 0f)
        {
            timeRemaining = 0f;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTimer(timeRemaining);
        }

        if (timeRemaining <= 0f)
        {
            EndMatch();
        }
    }

    public void StartMatch()
    {
        timeRemaining = matchDuration;
        matchActive = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetScore();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTimer(timeRemaining);
            UIManager.Instance.ShowStatus("Deliver packets to the relay!");
        }

        Debug.Log("Match started.");
    }

    private void EndMatch()
    {
        matchActive = false;

        int finalScore = 0;

        if (GameManager.Instance != null)
        {
            finalScore = GameManager.Instance.GetScore();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowStatus("Match Over! Final Score: " + finalScore);
        }

        Debug.Log("Match ended. Final Score: " + finalScore);
    }
}