using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "CoinToss/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public GameObject coinPrefabOverride; // 弾の形状を変える場合に使用
}
