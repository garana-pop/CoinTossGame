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

            // コインの速度を減衰させて器の上に留まりやすくする
            if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity *= 0.5f; // 50%に減衰
                rb.angularVelocity *= 0.5f;
                // 高速な移動による貫通を防止
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }

            // 親子関係を設定すると非一様スケールの影響で歪む可能性があるため、
            // 物理挙動のみで管理する（安定性が十分であれば親子関係は不要）
            // other.transform.SetParent(transform.parent);

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

            // 器から離れた際の物理設定の復元
            if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
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