using UnityEngine;
using System.Collections;

/// <summary>
/// カメラを揺らす演出を担当するコンポーネント。
/// </summary>
public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;
    private Coroutine currentShakeCoroutine;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    /// <summary>
    /// 指定された強度と時間でカメラを揺らす。
    /// </summary>
    /// <param name="duration">揺れる時間（秒）</param>
    /// <param name="magnitude">揺れの強度</param>
    public void Shake(float duration, float magnitude)
    {
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
            transform.localPosition = originalPosition;
        }
        currentShakeCoroutine = StartCoroutine(PerformShake(duration, magnitude));
    }

    private IEnumerator PerformShake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
        currentShakeCoroutine = null;
    }
}
