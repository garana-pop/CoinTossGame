// GameEventBus.cs
using System;

/// <summary>
/// ゲーム全体のイベント通知を一元管理する静的バスクラス。
/// 各クラスはこのクラスを経由して通知を送受信し、互いを直接参照しない。
/// </summary>
public static class GameEventBus
{
    // ---- イベント定義 ----
    public static event Action OnCoinThrown;     // コインを投擲したとき
    public static event Action<int> OnCoinLanded;     // 引数: 皿の上のコイン枚数
    public static event Action<int> OnScoreChanged;   // 引数: 現在の合計スコア
    public static event Action<int> OnEnemyDamaged;   // 引数: 残りHP
    public static event Action OnEnemyDefeated;  // 敵を倒したとき
    public static event Action OnPowerUpSelected; // パワーアップを選択したとき
    public static event Action OnWaveStarted;    // ウェーブが開始したとき

    // ---- Publishメソッド ----

    /// <summary>コイン投擲イベントを発火する</summary>
    public static void PublishCoinThrown()
        => OnCoinThrown?.Invoke();

    /// <summary>コイン着地イベントを発火する</summary>
    /// <param name="coinCount">現在皿の上にあるコイン枚数</param>
    public static void PublishCoinLanded(int coinCount)
        => OnCoinLanded?.Invoke(coinCount);

    /// <summary>スコア変化イベントを発火する</summary>
    /// <param name="totalScore">現在の合計スコア</param>
    public static void PublishScoreChanged(int totalScore)
        => OnScoreChanged?.Invoke(totalScore);

    /// <summary>敵ダメージイベントを発火する</summary>
    /// <param name="remaining">ダメージ後の敵残りHP</param>
    public static void PublishEnemyDamaged(int remaining)
        => OnEnemyDamaged?.Invoke(remaining);

    /// <summary>敵撃破イベントを発火する</summary>
    public static void PublishEnemyDefeated()
        => OnEnemyDefeated?.Invoke();

    /// <summary>パワーアップ選択完了イベントを発火する</summary>
    public static void PublishPowerUpSelected()
        => OnPowerUpSelected?.Invoke();

    /// <summary>ウェーブ開始イベントを発火する</summary>
    public static void PublishWaveStarted()
        => OnWaveStarted?.Invoke();
}