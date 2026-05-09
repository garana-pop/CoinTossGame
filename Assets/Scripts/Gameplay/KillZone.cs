using UnityEngine;

/// <summary>
/// 領域内に進入したコインを削除するクラス。
/// 主にステージ外に落下したコインの掃除に使用する。
/// </summary>
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
        }
    }
}
