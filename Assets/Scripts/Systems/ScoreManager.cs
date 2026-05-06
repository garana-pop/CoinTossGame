// ScoreManager.cs
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private bool debugMode;
    public int TotalScore { get; private set; } = 0;

    private void OnEnable() => GameEventBus.OnCoinLanded += HandleCoinLanded;
    private void OnDisable() => GameEventBus.OnCoinLanded -= HandleCoinLanded;

    private void HandleCoinLanded(int coinCount)
    {
        TotalScore = coinCount * GameConstants.SCORE_PER_COIN;
        GameEventBus.PublishScoreChanged(TotalScore);

        if (debugMode)
        {
            Debug.Log($"ScoreManager: TotalScore={TotalScore} (Coins={coinCount})");
        }
    }

    public void ResetScore()
    {
        TotalScore = 0;
        GameEventBus.PublishScoreChanged(TotalScore);
    }
}