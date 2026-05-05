// Vessel.cs
using UnityEngine;

/// <summary>
/// 皿の上にあるコイン枚数を管理し、変化のたびにイベントを発火するクラス。
/// コインの着地・落下を検知し、ScoreManagerへの直接参照は持たない。
/// </summary>
public class Vessel : MonoBehaviour
{
    // ---- インスペクター設定 ----
    [SerializeField] private bool debugMode; // デバッグログの有無

    // ---- 内部状態 ----
    private int coinCount = 0; // 現在皿の上にあるコイン枚数

    // -------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Coin")) return;

        // コインが皿に乗ったのでカウントを増やす
        coinCount++;
        GameEventBus.PublishCoinLanded(coinCount);

        if (debugMode)
        {
            Debug.Log($"{nameof(Vessel)}: コイン着地 枚数={coinCount}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Coin")) return;

        // コインが皿から落ちたのでカウントを減らす（0未満にはしない）
        coinCount = Mathf.Max(0, coinCount - 1);
        GameEventBus.PublishCoinLanded(coinCount);

        if (debugMode)
        {
            Debug.Log($"{nameof(Vessel)}: コイン落下 枚数={coinCount}");
        }
    }

    /// <summary>ウェーブ開始時に皿のコイン枚数カウントをリセットする</summary>
    public void ResetCoins() => coinCount = 0;
}