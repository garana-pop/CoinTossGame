using UnityEngine;

/// <summary>
/// ゲームの状態遷移を管理するクラス。
/// 状態の追加・遷移ルールの変更はこのクラスのみを修正する。
/// </summary>
public class GameStateMachine
{
    public enum State
    {
        Idle,           // 待機中
        Launching,      // コイン投入フェーズ（プレイヤーフェーズ）
        Judging,        // 静止判定中
        DamagePhase,    // プレイヤー攻撃（敵へのダメージ）
        EnemyPhase,     // 敵フェーズ（敵の攻撃）
        PowerUp,        // パワーアップ選択
        WaveTransit,    // ウェーブ移行中
        GameOver        // ゲームオーバー
    }

    public State Current { get; private set; } = State.Idle;

    public void Transition(State next)
    {
        if (Current == next) return;

        if (!IsValidTransition(Current, next))
        {
            Debug.LogWarning($"{nameof(GameStateMachine)}: 不正な遷移 {Current} -> {next}");
            return;
        }

        Debug.Log($"{nameof(GameStateMachine)}: {Current} -> {next}");
        Current = next;
    }

    private bool IsValidTransition(State from, State to) => (from, to) switch
    {
        (State.Idle, State.Launching) => true,
        (State.Launching, State.Judging) => true,
        (State.Judging, State.DamagePhase) => true,
        (State.DamagePhase, State.EnemyPhase) => true,
        (State.DamagePhase, State.PowerUp) => true,
        (State.EnemyPhase, State.Launching) => true,
        (State.EnemyPhase, State.GameOver) => true,
        (State.PowerUp, State.WaveTransit) => true,
        (State.WaveTransit, State.Launching) => true,
        (State.GameOver, State.Idle) => true,
        _ => false
    };
}