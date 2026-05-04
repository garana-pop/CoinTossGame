// GameManager.cs
using UnityEngine;

/// <summary>
/// ゲーム全体のフロー制御を担うクラス。
/// GameStateMachine と GameEventBus に処理を委譲し、他クラスへの直接参照を持たない。
/// </summary>
public class GameManager : MonoBehaviour
{
    // ---- 内部状態 ----
    private GameStateMachine stateMachine; // 状態管理の専任クラス

    // -------------------------------------------------------

    private void Awake() => stateMachine = new GameStateMachine();

    private void OnEnable()
    {
        GameEventBus.OnCoinThrown += HandleCoinThrown;
        GameEventBus.OnCoinLanded += HandleCoinLanded;
        GameEventBus.OnEnemyDefeated += HandleEnemyDefeated;
        GameEventBus.OnPowerUpSelected += HandlePowerUpSelected;
    }

    private void OnDisable()
    {
        GameEventBus.OnCoinThrown -= HandleCoinThrown;
        GameEventBus.OnCoinLanded -= HandleCoinLanded;
        GameEventBus.OnEnemyDefeated -= HandleEnemyDefeated;
        GameEventBus.OnPowerUpSelected -= HandlePowerUpSelected;
    }

    private void Start()
    {
        stateMachine.Transition(GameStateMachine.State.Launching);
        GameEventBus.PublishWaveStarted();
    }

    /// <summary>コイン投擲後の遷移処理（着地判定フェーズへ）</summary>
    private void HandleCoinThrown()
        => stateMachine.Transition(GameStateMachine.State.Judging);

    /// <summary>コイン着地後の遷移処理（スコア表示フェーズへ）</summary>
    /// <param name="coinCount">現在皿の上にあるコイン枚数</param>
    private void HandleCoinLanded(int coinCount)
        => stateMachine.Transition(GameStateMachine.State.ShowingScore);

    /// <summary>敵撃破後の遷移処理</summary>
    private void HandleEnemyDefeated()
        => stateMachine.Transition(GameStateMachine.State.PowerUp);

    /// <summary>パワーアップ選択後の遷移処理</summary>
    private void HandlePowerUpSelected()
    {
        stateMachine.Transition(GameStateMachine.State.WaveTransit);
        stateMachine.Transition(GameStateMachine.State.Launching);
        GameEventBus.PublishWaveStarted();
    }
}