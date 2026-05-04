// CoinLauncher.cs
using UnityEngine;

/// <summary>
/// マウスホイールでチャージし、左クリックでコインを投擲するクラス。
/// 投擲責任のみを持ち、着地判定はVessel側に委譲する。
/// </summary>
public class CoinLauncher : MonoBehaviour
{
    // ---- インスペクター設定 ----
    [SerializeField] private GameObject coinPrefab;       // 投擲するコインのPrefab
    [SerializeField] private Transform launchPoint;       // コインを生成する発射位置
    [SerializeField] private bool debugMode;              // デバッグログの有無

    // ---- 定数 ----
    private const float CHARGE_DISPLAY_SCALE = 10f;      // チャージ量をUI表示用に変換するスケール

    // ---- 内部状態 ----
    private float chargeAmount;                          // 現在のチャージ量

    // ---- プロパティ ----
    /// <summary>現在のチャージ量を0〜1で返す（UI進捗バー向け）</summary>
    public float ChargeRatio => chargeAmount / GameConstants.MAX_LAUNCH_FORCE;

    // -------------------------------------------------------

    private void Update()
    {
        // チャージ中でないときだけ入力を受け付ける（連続投擲防止）
        HandleChargeInput();
        HandleLaunchInput();
    }

    /// <summary>マウスホイールでチャージ量を増減する</summary>
    private void HandleChargeInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // ホイール入力取得
        chargeAmount += scroll * CHARGE_DISPLAY_SCALE;

        // チャージ量を最小〜最大の範囲にクランプ
        chargeAmount = Mathf.Clamp(
            chargeAmount,
            GameConstants.MIN_LAUNCH_FORCE,
            GameConstants.MAX_LAUNCH_FORCE
        );

        if (debugMode)
        {
            Debug.Log($"{nameof(CoinLauncher)}: チャージ量 = {chargeAmount:F2}");
        }
    }

    /// <summary>左クリックでコインを発射する（チャージ量 > 0 のときのみ）</summary>
    private void HandleLaunchInput()
    {
        if (Input.GetMouseButtonDown(0) && chargeAmount > 0f)
        {
            LaunchCoin();
        }
    }

    /// <summary>チャージ量に応じた力でコインを発射し、チャージをリセットする</summary>
    private void LaunchCoin()
    {
        // コインPrefabとlaunchPointが未設定の場合は早期リターン
        if (coinPrefab == null)
        {
            Debug.LogError($"{nameof(CoinLauncher)}: coinPrefab が未設定です。");
            return;
        }
        if (launchPoint == null)
        {
            Debug.LogError($"{nameof(CoinLauncher)}: launchPoint が未設定です。");
            return;
        }

        // コインを生成して発射方向へ力を加える
        GameObject coin = Instantiate(coinPrefab, launchPoint.position, launchPoint.rotation);

        if (coin.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            // launchPointの前方向を発射方向として使用
            rb.AddForce(launchPoint.forward * chargeAmount, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError($"{nameof(CoinLauncher)}: コインPrefabにRigidbodyが見つかりません。");
        }

        if (debugMode)
        {
            Debug.Log($"{nameof(CoinLauncher)}: コイン発射 力 = {chargeAmount:F2}");
        }

        // チャージリセット（MIN_LAUNCH_FORCEに戻すことで次の入力に備える）
        chargeAmount = 0f;
    }
}