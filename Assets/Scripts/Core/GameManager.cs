using UnityEngine;

/// <summary>
/// ゲーム全体のフロー制御を担うクラス。
/// GameStateMachine と GameEventBus に処理を委譲し、他クラスへの直接参照を持たない。
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private bool debugMode; // デバッグモードフラグ

    private GameStateMachine stateMachine; // 状態管理

    private void Awake() => stateMachine = new GameStateMachine();

    private void OnEnable()
    {
        GameEventBus.OnCoinLanded += HandleCoinLanded;
        GameEventBus.OnEnemyDefeated += HandleEnemyDefeated;
        GameEventBus.OnPowerUpSelected += HandlePowerUpSelected;
    }

    private void OnDisable()
    {
        GameEventBus.OnCoinLanded -= HandleCoinLanded;
        GameEventBus.OnEnemyDefeated -= HandleEnemyDefeated;
        GameEventBus.OnPowerUpSelected -= HandlePowerUpSelected;
    }

    private void Start()
    {
        // ゲーム開始時に投擲状態へ遷移しウェーブを開始する
        stateMachine.Transition(GameStateMachine.State.Launching);
        GameEventBus.PublishWaveStarted();

        if (debugMode)
        {
            Debug.Log($"{nameof(GameManager)}: ゲーム開始 / 初期ウェーブ発火");
        }
    }

    /// <summary>コイン着地後の遷移処理</summary>
    /// <param name="score">着地で得たスコア</param>
    private void HandleCoinLanded(int score)
    {
        stateMachine.Transition(GameStateMachine.State.ShowingScore);

        if (debugMode)
        {
            Debug.Log($"{nameof(GameManager)}: コイン着地 / score={score}");
        }
    }

    /// <summary>敵撃破後の遷移処理</summary>
    private void HandleEnemyDefeated()
    {
        stateMachine.Transition(GameStateMachine.State.PowerUp);

        if (debugMode)
        {
            Debug.Log($"{nameof(GameManager)}: 敵撃破 / パワーアップ選択へ遷移");
        }
    }

    /// <summary>パワーアップ選択後の遷移処理</summary>
    private void HandlePowerUpSelected()
    {
        stateMachine.Transition(GameStateMachine.State.WaveTransit);
        stateMachine.Transition(GameStateMachine.State.Launching);
        GameEventBus.PublishWaveStarted();

        if (debugMode)
        {
            Debug.Log($"{nameof(GameManager)}: パワーアップ選択完了 / 次ウェーブ発火");
        }
    }
}