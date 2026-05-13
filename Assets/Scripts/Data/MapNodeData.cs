using UnityEngine;

[CreateAssetMenu(fileName = "MapNodeData", menuName = "CoinToss/MapNodeData")]
public class MapNodeData : ScriptableObject
{
    public enum NodeType
    {
        Battle,
        Store,
        Event,
        Boss
    }

    public NodeType nodeType;
    public string nodeName;
    public Sprite icon;
}
