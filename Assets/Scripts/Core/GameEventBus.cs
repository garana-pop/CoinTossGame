using System;

/// <summary>
/// ゲーム全体のイベント通知を管理する静的クラス。
/// クラス間の直接参照を排除し、すべての通知はここを経由させる。
/// </summary>
public static class GameEventBus
{
    public static event Action<int> OnCoinLanded;    // コインが着地したときに発火（引数: スコア加算量）
    public static event Action<int> OnEnemyDamaged;  // 敵がダメージを受けたときに発火（引数: 残りHP）
    public static event Action OnEnemyDefeated; // 敵を倒したときに発火
    public static event Action OnPowerUpSelected; // パワーアップを選択したときに発火
    public static event Action OnWaveStarted;   // ウェーブが開始したときに発火

    /// <summary>コイン着地イベントを発火する</summary>
    /// <param name="score">着地で得たスコア加算量</param>
    public static void PublishCoinLanded(int score) => OnCoinLanded?.Invoke(score);

    /// <summary>敵ダメージイベントを発火する</summary>
    /// <param name="remaining">ダメージ後の敵の残りHP</param>
    public static void PublishEnemyDamaged(int remaining) => OnEnemyDamaged?.Invoke(remaining);

    /// <summary>敵撃破イベントを発火する</summary>
    public static void PublishEnemyDefeated() => OnEnemyDefeated?.Invoke();

    /// <summary>パワーアップ選択イベントを発火する</summary>
    public static void PublishPowerUpSelected() => OnPowerUpSelected?.Invoke();

    /// <summary>ウェーブ開始イベントを発火する</summary>
    public static void PublishWaveStarted() => OnWaveStarted?.Invoke();
}