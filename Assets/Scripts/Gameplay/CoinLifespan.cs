using UnityEngine;

/// <summary>
/// 生成されてから一定時間経過したコインを自動的に削除するクラス。
/// 器に入った場合は削除を停止する。
/// </summary>
public class CoinLifespan : MonoBehaviour
{
    private float timer = 0f;
    private bool isTracking = true;

    private void Update()
    {
        if (!isTracking) return;

        timer += Time.deltaTime;
        
        if (timer >= GameConstants.COIN_LIFESPAN)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 寿命による削除を停止する（器に入った際などに呼び出す）
    /// </summary>
    public void StopLifespan()
    {
        isTracking = false;
    }

    /// <summary>
    /// 寿命による削除を再開する（器から出た際などに呼び出す）
    /// </summary>
    public void StartLifespan()
    {
        isTracking = true;
    }
}
