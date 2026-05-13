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
    
    public static event Action OnEnemyAttackStarted;  // 敵の攻撃開始
    public static event Action OnEnemyAttackFinished; // 敵の攻撃終了
    
    public static event Action<int, int> OnPlayerDamaged;  // プレイヤーダメージ: (ダメージ量, 残りHP)
    public static event Action OnPlayerDefeated; // プレイヤーが倒れたとき

    public static event Action<int> OnMoneyChanged; // 所持金が変更されたとき: 現在の金額
    public static event Action OnItemObtained; // アイテムを獲得したとき

    public static event Action OnPowerUpSelected; // パワーアップを選択したとき
    public static event Action OnWaveStarted;    // ウェーブが開始されたとき
    
    public static event Action<float> OnTimerUpdated; // 制限時間更新: 残り時間

    // ---- Publishメソッド ----

    /// <summary>
    /// コインを投擲したことを購読者に通知します。
    /// </summary>
    public static void PublishCoinThrown() => OnCoinThrown?.Invoke();

    /// <summary>
    /// コインが着地したことを購読者に通知します。
    /// </summary>
    /// <param name="coinCount">器の上に残っているコインの枚数</param>
    public static void PublishCoinLanded(int coinCount) => OnCoinLanded?.Invoke(coinCount);

    /// <summary>
    /// スコアの合計が更新されたことを購読者に通知します。
    /// </summary>
    /// <param name="totalScore">現在の合計スコア</param>
    public static void PublishScoreChanged(int totalScore) => OnScoreChanged?.Invoke(totalScore);

    /// <summary>
    /// 敵がダメージを受けたことを購読者に通知します。
    /// </summary>
    /// <param name="damage">与えたダメージ量</param>
    /// <param name="remaining">敵の残りHP</param>
    public static void PublishEnemyDamaged(int damage, int remaining) => OnEnemyDamaged?.Invoke(damage, remaining);

    /// <summary>
    /// 敵を倒したことを購読者に通知します。
    /// </summary>
    public static void PublishEnemyDefeated() => OnEnemyDefeated?.Invoke();

    /// <summary>
    /// 敵の攻撃開始を購読者に通知します。
    /// </summary>
    public static void PublishEnemyAttackStarted() => OnEnemyAttackStarted?.Invoke();

    /// <summary>
    /// 敵の攻撃終了を購読者に通知します。
    /// </summary>
    public static void PublishEnemyAttackFinished() => OnEnemyAttackFinished?.Invoke();

    /// <summary>
    /// プレイヤーがダメージを受けたことを購読者に通知します。
    /// </summary>
    /// <param name="damage">受けたダメージ量</param>
    /// <param name="remaining">プレイヤーの残りHP</param>
    public static void PublishPlayerDamaged(int damage, int remaining) => OnPlayerDamaged?.Invoke(damage, remaining);

    /// <summary>
    /// プレイヤーが倒れたことを購読者に通知します。
    /// </summary>
    public static void PublishPlayerDefeated() => OnPlayerDefeated?.Invoke();

    /// <summary>
    /// 所持金が変更されたことを通知します。
    /// </summary>
    public static void PublishMoneyChanged(int totalMoney) => OnMoneyChanged?.Invoke(totalMoney);

    /// <summary>
    /// アイテムを獲得したことを通知します。
    /// </summary>
    public static void PublishItemObtained() => OnItemObtained?.Invoke();

    /// <summary>
    /// パワーアップが選択されたことを購読者に通知します。
/// </summary>
    public static void PublishPowerUpSelected() => OnPowerUpSelected?.Invoke();

    /// <summary>
    /// ウェーブが開始されたことを購読者に通知します。
    /// </summary>
    public static void PublishWaveStarted() => OnWaveStarted?.Invoke();

    /// <summary>
    /// 残り時間の更新を購読者に通知します。
    /// </summary>
    /// <param name="remainingTime">残り時間（秒）</param>
    public static void PublishTimerUpdated(float remainingTime) => OnTimerUpdated?.Invoke(remainingTime);
}