// ScoreManager.cs
using UnityEngine;

/// <summary>
/// コイン枚数を受け取りスコアを管理するクラス。
/// OnCoinLanded を購読し、SCORE_PER_COIN を掛けて累計スコアを更新する。
/// スコア更新時は OnScoreChanged をUI向けに発火する。
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // ---- インスペクター設定 ----
    [SerializeField] private bool debugMode; // デバッグログの有無

    // ---- プロパティ ----
    public int TotalScore { get; private set; } = 0; // 累計スコア

    // -------------------------------------------------------

    private void OnEnable() => GameEventBus.OnCoinLanded += HandleCoinLanded;
    private void OnDisable() => GameEventBus.OnCoinLanded -= HandleCoinLanded;

    /// <summary>
    /// コイン枚数からスコアを算出して累計スコアを更新する。
    /// 算出したスコアは OnScoreChanged イベントで通知する。
    /// </summary>
    /// <param name="coinCount">皿の上の現在のコイン枚数</param>
    private void HandleCoinLanded(int coinCount)
    {
        // 今回の投擲で得るスコア = 現在の枚数 × 1枚あたりのスコア
        int gainedScore = coinCount * GameConstants.SCORE_PER_COIN;
        TotalScore += gainedScore;

        // UIへスコア更新を通知
        GameEventBus.PublishScoreChanged(TotalScore);

        if (debugMode)
        {
            Debug.Log($"{nameof(ScoreManager)}: +{gainedScore} 合計={TotalScore} (枚数={coinCount})");
        }
    }

    /// <summary>
    /// ウェーブ開始時に累計スコアをリセットする。
    /// 呼び出し元：WaveManager
    /// </summary>
    public void ResetScore()
    {
        TotalScore = 0;

        // リセット後のスコアをUIへ通知
        GameEventBus.PublishScoreChanged(TotalScore);

        if (debugMode)
        {
            Debug.Log($"{nameof(ScoreManager)}: スコアをリセットしました");
        }
    }
}