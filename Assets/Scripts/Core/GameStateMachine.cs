using UnityEngine;

/// <summary>
/// ゲームの状態遷移を管理するクラス。
/// 状態の追加・遷移ルールの変更はこのクラスのみを修正する。
/// </summary>
public class GameStateMachine
{
    // ゲームの状態を表す列挙型
    public enum State
    {
        Idle,         // 待機中
        Launching,    // コイン投擲中
        Judging,      // 着地判定中
        ShowingScore, // スコア表示中
        PowerUp,      // パワーアップ選択中
        WaveTransit   // ウェーブ移行中
    }

    public State Current { get; private set; } = State.Idle; // 現在の状態

    /// <summary>指定した状態へ遷移する。不正遷移はログを出してスキップする。</summary>
    /// <param name="next">遷移先の状態</param>
    public void Transition(State next)
    {
        if (!IsValidTransition(Current, next))
        {
            Debug.LogWarning($"{nameof(GameStateMachine)}: 無効な遷移 {Current} → {next}");
            return;
        }

        // 遷移ログの出力
        Debug.Log($"{nameof(GameStateMachine)}: {Current} → {next}");
        Current = next;
    }

    /// <summary>遷移の合法性を検証する。新しい状態を追加する場合はここに追記する。</summary>
    /// <param name="from">遷移元の状態</param>
    /// <param name="to">遷移先の状態</param>
    /// <returns>遷移が合法であれば true、不正であれば false</returns>
    private bool IsValidTransition(State from, State to) => (from, to) switch
    {
        (State.Idle, State.Launching) => true,
        (State.Launching, State.Judging) => true,
        (State.Judging, State.ShowingScore) => true,
        (State.ShowingScore, State.Launching) => true,
        (State.ShowingScore, State.PowerUp) => true,
        (State.PowerUp, State.WaveTransit) => true,
        (State.WaveTransit, State.Launching) => true,
        _ => false
    };
}