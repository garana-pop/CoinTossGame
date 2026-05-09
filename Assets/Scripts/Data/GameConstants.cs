/// <summary>
/// ゲーム全体で使用するマジックナンバーを集約する定数クラス。
/// </summary>
public static class GameConstants
{
    // ---- 投擲物理 ----
    public const float MIN_LAUNCH_FORCE = 3f;
    public const float MAX_LAUNCH_FORCE = 15f;
    public const float WHEEL_VELOCITY_THRESHOLD = 1.5f;
    public const float WHEEL_VELOCITY_TO_FORCE_SCALE = 5f;

    // ---- 制限時間 ----
    public const float TURN_TIME_LIMIT = 10f; // プレイヤーの投擲時間

    // ---- パワーアップ ----
    public const int POWERUP_CHOICE_COUNT = 3;

    // ---- 敵・プレイヤーHP ----
    public const int PLAYER_MAX_HP = 100;
    public const int ENEMY_BASE_HP = 100;
    public const float ENEMY_HP_SCALE_PER_WAVE = 0.2f;
    public const int ENEMY_ATTACK_DAMAGE = 15;

    // ---- スコア・ダメージ ----
    public const int SCORE_PER_COIN = 10; // 器に乗ったコイン1枚あたりのダメージ
    public const float COIN_LIFESPAN = 15.0f; // 器に入らなかったコインの消滅時間

    // ---- UI演出用ウェイト ----
    public const float JUDGE_DURATION = 2.0f; // 静止判定待機時間
    public const float DISPLAY_DURATION = 1.5f; // スコア/ダメージ表示時間
    public const float WAVE_TRANSIT_DURATION = 1.0f;
}