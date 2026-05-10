using UnityEngine;

/// <summary>
/// コインの衝突時に効果音を鳴らすコンポーネント。
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class CoinCollisionFeedback : MonoBehaviour
{
    [SerializeField] private AudioClip[] reflectionSounds;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;
    [SerializeField] private float minCollisionVelocity = 0.5f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 衝突相手の名前に "Wall" が含まれているかチェック
        if (collision.gameObject.name.Contains("Wall"))
        {
            // 衝突速度が一定以上の場合のみ再生
            if (collision.relativeVelocity.magnitude > minCollisionVelocity)
            {
                PlayReflectionSound();
            }
        }
    }

    private void PlayReflectionSound()
    {
        if (reflectionSounds == null || reflectionSounds.Length == 0) return;

        AudioClip clip = reflectionSounds[Random.Range(0, reflectionSounds.Length)];
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(clip);
    }
}
