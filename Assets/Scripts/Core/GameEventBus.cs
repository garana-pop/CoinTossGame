using System;

/// <summary>
/// ゲーム全体のイベント通知を一括管理する静的バスクラス。
/// </summary>
public static class GameEventBus
{
    // ---- イベント定義 ----
    public static event Action OnCoinThrown;     // コインを投擲したとき
    public static event Action<int> OnCoinLanded;     // 着地: 器の上のコイン枚数
    public static event Action<int> OnScoreChanged;   // 更新: 現在の合計スコア
    
    public static event Action<int, int> OnEnemyDamaged;   // 敵ダメージ: (ダメージ量, 残りHP)
    public static event Action OnEnemyDefeated;  // 敵を倒したとき
    
    public static event Action<int, int> OnPlayerDamaged;  // プレイヤーダメージ: (ダメージ量, 残りHP)
    public static event Action OnPlayerDefeated; // プレイヤーが倒れたとき

    public static event Action OnPowerUpSelected; // パワーアップを選択したとき
    public static event Action OnWaveStarted;    // ウェーブが開始されたとき
    
    public static event Action<float> OnTimerUpdated; // 制限時間更新: 残り時間

    // ---- Publishメソッド ----

    public static void PublishCoinThrown() => OnCoinThrown?.Invoke();

    public static void PublishCoinLanded(int coinCount) => OnCoinLanded?.Invoke(coinCount);

    public static void PublishScoreChanged(int totalScore) => OnScoreChanged?.Invoke(totalScore);

    public static void PublishEnemyDamaged(int damage, int remaining) => OnEnemyDamaged?.Invoke(damage, remaining);

    public static void PublishEnemyDefeated() => OnEnemyDefeated?.Invoke();

    public static void PublishPlayerDamaged(int damage, int remaining) => OnPlayerDamaged?.Invoke(damage, remaining);

    public static void PublishPlayerDefeated() => OnPlayerDefeated?.Invoke();

    public static void PublishPowerUpSelected() => OnPowerUpSelected?.Invoke();

    public static void PublishWaveStarted() => OnWaveStarted?.Invoke();
    
    public static void PublishTimerUpdated(float remainingTime) => OnTimerUpdated?.Invoke(remainingTime);
}