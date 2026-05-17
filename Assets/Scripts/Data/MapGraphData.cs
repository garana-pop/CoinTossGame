using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapNodeInstance
{
    public int nodeID;
    public MapNodeData.NodeType nodeType;
    public Vector2 position;
    public List<int> connectedTo = new List<int>();
    public int layer;
    public bool isVisited;
}

[System.Serializable]
public class MapGraphData
{
    public List<MapNodeInstance> nodes = new List<MapNodeInstance>();
    public int currentNodeID = -1; // -1 indicates player hasn't selected a starting node yet
}
