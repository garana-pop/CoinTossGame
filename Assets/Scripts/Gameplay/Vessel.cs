using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 器の上に乗り、器の中に留まっているコインを管理するクラス。
/// トリガー内に存在するコインをHashSetで管理し、正確なカウントを維持する。
/// </summary>
public class Vessel : MonoBehaviour
{
    // ---- インスペクター設定 ----
    [SerializeField] private bool debugMode = true;

    // ---- 内部状態 ----
    private readonly HashSet<Collider> coinsInVessel = new HashSet<Collider>();

    // -------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Coin")) return;

        if (coinsInVessel.Add(other))
        {
            GameEventBus.PublishCoinLanded(coinsInVessel.Count);

            if (debugMode)
            {
                Debug.Log($"{nameof(Vessel)}: コイン進入 現在の数量={coinsInVessel.Count}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Coin")) return;

        if (coinsInVessel.Remove(other))
        {
            GameEventBus.PublishCoinLanded(coinsInVessel.Count);

            if (debugMode)
            {
                Debug.Log($"{nameof(Vessel)}: コイン脱出 現在の数量={coinsInVessel.Count}");
            }
        }
    }

    /// <summary>現在のコイン数を取得</summary>
    public int GetCoinCount() => coinsInVessel.Count;

    /// <summary>ウェーブ開始時などにカウントをクリアする</summary>
    public void ResetCoins()
    {
        coinsInVessel.Clear();
        GameEventBus.PublishCoinLanded(0);
    }
}