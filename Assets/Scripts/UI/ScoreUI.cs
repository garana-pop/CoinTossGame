// ScoreUI.cs
using TMPro;
using UnityEngine;

/// <summary>
/// GameEventBus.OnScoreChanged を購読し、スコアテキストを更新するUIクラス。
/// スコアの計算には関与せず、表示のみを担当する。
/// </summary>
public class ScoreUI : MonoBehaviour
{
    // ---- インスペクター設定 ----
    [SerializeField] private TextMeshProUGUI scoreText; // スコアを表示するテキスト
    [SerializeField] private bool debugMode;            // デバッグログの有無

    // ---- 定数 ----
    private const string SCORE_PREFIX = "Score: "; // スコア表示の接頭辞

    // -------------------------------------------------------

    private void OnEnable() => GameEventBus.OnScoreChanged += HandleScoreChanged;
    private void OnDisable() => GameEventBus.OnScoreChanged -= HandleScoreChanged;

    private void Start()
    {
        // scoreText が未設定の場合は早期リターン
        if (scoreText == null)
        {
            Debug.LogError($"{nameof(ScoreUI)}: scoreText が未設定です。");
            return;
        }

        // 初期表示をリセット
        scoreText.text = SCORE_PREFIX + "0";
    }

    /// <summary>スコアが更新されたときにテキストを書き換える</summary>
    /// <param name="totalScore">現在の合計スコア</param>
    private void HandleScoreChanged(int totalScore)
    {
        if (scoreText == null) return;

        scoreText.text = SCORE_PREFIX + totalScore;

        if (debugMode)
        {
            Debug.Log($"{nameof(ScoreUI)}: スコア表示更新 → {totalScore}");
        }
    }
}