using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score")]
    public int score = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        if (MatchManager.Instance != null && !MatchManager.Instance.IsMatchActive)
        {
            return;
        }

        score += amount;
        UpdateScoreUI();

        Debug.Log("Score: " + score);
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(score);
        }
    }
}