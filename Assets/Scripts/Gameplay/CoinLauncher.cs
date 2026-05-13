// CoinLauncher.cs
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// マウスホイールの速度を検知し、閾値を超えたとき自動でコインを投擲するクラス。
/// 投擲責任のみを持ち、着地判定はVessel側に委譲する。
/// ※※マウスホイールを前に回したときのみ、コインを投擲するように修正が必要※※
/// </summary>
public class CoinLauncher : MonoBehaviour
{
    // ---- インスペクター設定 ----
    [SerializeField] private GameObject coinPrefab;  // 投擲するコインのPrefab
    [SerializeField] private Transform launchPoint; // コインを生成する発射位置
    [SerializeField, Range(0f, 0.5f)] private float jitterAmount = 0.05f; // 投擲方向のランダムな揺れ
    [SerializeField] private bool debugMode;   // デバッグログの有無

    [Header("Effects Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] shotSounds;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private float minShakeMagnitude = 0.05f;
    [SerializeField] private float maxShakeMagnitude = 0.2f;
    [SerializeField] private float shakeDuration = 0.1f;

    // ---- 内部状態 ----
private float peakWheelVelocity = 0f;  // スクロール中に記録した最大速度
    private bool wasScrolling = false; // 前フレームにスクロールしていたか
    private int coinsPerLaunch = 1; // 1回の操作で発射されるコインの数

    // -------------------------------------------------------

    public void SetCoinsPerLaunch(int count) => coinsPerLaunch = count;
    public void AddCoinsPerLaunch(int amount) => coinsPerLaunch += amount;
    public void ChangeCoinPrefab(GameObject newPrefab)
    {
        coinPrefab = newPrefab;
        if (RunManager.Instance != null) RunManager.Instance.CurrentCoinPrefab = newPrefab;
    }

    private void Start()
    {
        if (RunManager.Instance != null && RunManager.Instance.CurrentCoinPrefab != null)
        {
            coinPrefab = RunManager.Instance.CurrentCoinPrefab;
        }
    }

    private void Update()
{
        // 新Input Systemでのホイール取得
        float scroll = Mouse.current.scroll.ReadValue().y / 120f;

        // 企画書に従い、前方向（正の値）のスクロールのみを検知するように制限
        if (scroll > 0f)
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
    /// <param name="scroll">今フレームのスクロール量</param>
    private void HandleScrolling(float scroll)
    {
        // 速度（絶対値）を算出
        float currentVelocity = scroll / Time.deltaTime;

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

        // 複数枚発射に対応
        StartCoroutine(LaunchMultipleCoins(launchForce));
    }

    private IEnumerator LaunchMultipleCoins(float force)
    {
        for (int i = 0; i < coinsPerLaunch; i++)
        {
            LaunchCoin(force);
            if (coinsPerLaunch > 1) yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>指定した力でコインを発射し、投擲イベントを発火する</summary>
    /// <param name="force">投擲力</param>
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

        // コインを生成し、launchPoint の前方向（揺れあり）へ力を加える
        GameObject coin = Instantiate(coinPrefab, launchPoint.position, launchPoint.rotation);

        // 発射方向にランダムな揺れを加える
        Vector3 launchDirection = launchPoint.forward;
        if (jitterAmount > 0f)
        {
            Vector3 jitter = Random.insideUnitSphere * jitterAmount;
            launchDirection = (launchDirection + jitter).normalized;
        }

        if (coin.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(launchDirection * force, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError($"{nameof(CoinLauncher)}: コインPrefabにRigidbodyが見つかりません。");
        }

        // 演出：発射音とカメラシェイク
        PlayLaunchEffects(force);

        // 投擲イベントを発火（GameManager が Judging へ遷移する）
GameEventBus.PublishCoinThrown();

        if (debugMode)
        {
            Debug.Log($"{nameof(CoinLauncher)}: 投擲 force={force:F2}");
        }
        }

        /// <summary>
        /// 発射時の演出（音声・カメラシェイク）を実行する。
        /// </summary>
        /// <param name="force">発射力</param>
        private void PlayLaunchEffects(float force)
        {
        // 力の割合（0.0 ~ 1.0）を計算
        float forcePercent = Mathf.InverseLerp(GameConstants.MIN_LAUNCH_FORCE, GameConstants.MAX_LAUNCH_FORCE, force);

        // 発射音の再生
        if (audioSource != null && shotSounds != null && shotSounds.Length > 0)
        {
            AudioClip clip = shotSounds[Random.Range(0, shotSounds.Length)];
            audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, forcePercent);
            audioSource.PlayOneShot(clip);
        }

        // カメラシェイク
        if (cameraShake != null)
        {
            float magnitude = Mathf.Lerp(minShakeMagnitude, maxShakeMagnitude, forcePercent);
            cameraShake.Shake(shakeDuration, magnitude);
        }

        // マズルフラッシュ（視覚効果）
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        }

        /// <summary>スクロール状態をリセットする（停止検知後に呼び出す）</summary>
private void ResetScrollState()
    {
        peakWheelVelocity = 0f;
        wasScrolling = false;
    }
}