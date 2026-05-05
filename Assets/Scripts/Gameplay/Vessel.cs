using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 器の上に乗り、器の中に留まっているコインを管理するクラス。
/// トリガー内に存在するコインをHashSetで管理し、正確なカウントを維持する。
/// コインが器の上に入った際、親子関係を設定して安定性を高める（物理挙動は維持）。
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

            // コインの速度を少し減衰させて器の上に留まりやすくする（任意）
            if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity *= 0.8f;
                rb.angularVelocity *= 0.8f;
                // 高速な移動による貫通を防ぐ
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }

            // 器（親オブジェクト）の子供に設定して、相対的な安定性を高める
            // ただし Rigidbody は dynamic のままなので、押し出されることは可能
            other.transform.SetParent(transform.parent);

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

            // 器から離れたら親子関係を解除
            if (other.transform.parent == transform.parent)
            {
                other.transform.SetParent(null);
                if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                }
            }

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