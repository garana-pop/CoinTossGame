using UnityEngine;

/// <summary>
/// ステージ全体の物理設定（特に反発係数）を管理するクラス。
/// インスペクターからリアルタイムに反発係数を調整し、指定された PhysicsMaterial に反映します。
/// </summary>
[ExecuteAlways]
public class StageConfig : MonoBehaviour
{
    [Header("物理設定")]
    [Tooltip("ステージ全体の反発係数 (0: 跳ねない, 1: 完全に跳ねる)")]
    [Range(0f, 1f)]
    [SerializeField] private float bounciness = 0.6f;

    [Header("参照アセット")]
    [SerializeField] private PhysicsMaterial targetMaterial;

    private void Update()
    {
        ApplyBounciness();
    }

    private void OnValidate()
    {
        ApplyBounciness();
    }

    /// <summary>
    /// 指定されたマテリアルに現在の反発係数を適用します。
    /// </summary>
    private void ApplyBounciness()
    {
        if (targetMaterial != null)
        {
            if (!Mathf.Approximately(targetMaterial.bounciness, bounciness))
            {
                targetMaterial.bounciness = bounciness;
            }
        }
    }
}
