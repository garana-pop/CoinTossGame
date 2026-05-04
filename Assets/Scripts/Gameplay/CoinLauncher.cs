// CoinLauncher.cs
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// マウスホイールの速度を検知し、閾値を超えたとき自動でコインを投擲するクラス。
/// 投擲責任のみを持ち、着地判定はVessel側に委譲する。
/// </summary>
public class CoinLauncher : MonoBehaviour
{
    // ---- インスペクター設定 ----
    [SerializeField] private GameObject coinPrefab;  // 投擲するコインのPrefab
    [SerializeField] private Transform launchPoint; // コインを生成する発射位置
    [SerializeField] private bool debugMode;   // デバッグログの有無

    // ---- 内部状態 ----
    private float peakWheelVelocity = 0f;  // スクロール中に記録した最大速度（絶対値）
    private bool wasScrolling = false; // 前フレームにスクロールしていたか

    // -------------------------------------------------------

    private void Update()
    {
        // 新Input Systemでのホイール取得
        float scroll = Mouse.current.scroll.ReadValue().y / 120f;

        if (scroll != 0f)
        {
            HandleScrolling(scroll);
        }
        else if (wasScrolling)
        {
            TryLaunchCoin();
            ResetScrollState();
        }
    }

    /// <summary>スクロール中のホイール速度を計測し、ピーク値を更新する</summary>
    /// <param name="scroll">今フレームのスクロール量（Mouse ScrollWheel）</param>
    private void HandleScrolling(float scroll)
    {
        // delta / deltaTime で 1秒あたりの速度（絶対値）を算出
        float currentVelocity = Mathf.Abs(scroll) / Time.deltaTime;

        // 最大速度を更新
        peakWheelVelocity = Mathf.Max(peakWheelVelocity, currentVelocity);
        wasScrolling = true;

        if (debugMode)
        {
            Debug.Log($"{nameof(CoinLauncher)}: ホイール速度 {currentVelocity:F2} / ピーク {peakWheelVelocity:F2}");
        }
    }

    /// <summary>ピーク速度が閾値を超えていればコインを投擲する。閾値未満の場合はスキップ。</summary>
    private void TryLaunchCoin()
    {
        if (peakWheelVelocity < GameConstants.WHEEL_VELOCITY_THRESHOLD)
        {
            if (debugMode)
            {
                Debug.Log($"{nameof(CoinLauncher)}: 速度不足のため投擲しない ({peakWheelVelocity:F2})");
            }
            return;
        }

        // ピーク速度を変換係数でスケーリングし、投擲力の範囲にクランプ
        float launchForce = Mathf.Clamp(
            peakWheelVelocity * GameConstants.WHEEL_VELOCITY_TO_FORCE_SCALE,
            GameConstants.MIN_LAUNCH_FORCE,
            GameConstants.MAX_LAUNCH_FORCE
        );

        LaunchCoin(launchForce);
    }

    /// <summary>指定した力でコインを発射し、投擲イベントを発火する</summary>
    /// <param name="force">投擲力（MIN_LAUNCH_FORCE ～ MAX_LAUNCH_FORCE）</param>
    private void LaunchCoin(float force)
    {
        // coinPrefab が未設定の場合は早期リターン
        if (coinPrefab == null)
        {
            Debug.LogError($"{nameof(CoinLauncher)}: coinPrefab が未設定です。");
            return;
        }

        // launchPoint が未設定の場合は早期リターン
        if (launchPoint == null)
        {
            Debug.LogError($"{nameof(CoinLauncher)}: launchPoint が未設定です。");
            return;
        }

        // コインを生成し、launchPoint の前方向へ力を加える
        GameObject coin = Instantiate(coinPrefab, launchPoint.position, launchPoint.rotation);

        if (coin.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(launchPoint.forward * force, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError($"{nameof(CoinLauncher)}: コインPrefabにRigidbodyが見つかりません。");
        }

        // 投擲イベントを発火（GameManager が Judging へ遷移する）
        GameEventBus.PublishCoinThrown();

        if (debugMode)
        {
            Debug.Log($"{nameof(CoinLauncher)}: 投擲 force={force:F2}");
        }
    }

    /// <summary>スクロール状態をリセットする（停止検知後に呼び出す）</summary>
    private void ResetScrollState()
    {
        peakWheelVelocity = 0f;
        wasScrolling = false;
    }
}