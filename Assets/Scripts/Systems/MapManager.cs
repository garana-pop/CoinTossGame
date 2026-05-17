using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform contentParent;
    public GameObject nodePrefab;
    public GameObject linePrefab;

    [Header("Data")]
    public List<MapNodeData> allNodeData;

    private Dictionary<int, MapNode> instantiatedNodes = new Dictionary<int, MapNode>();

    private void Start()
    {
        if (RunManager.Instance == null || RunManager.Instance.currentMap == null)
        {
            Debug.LogError("RunManager or MapData is missing!");
            return;
        }

        GenerateMapUI();
        AdjustContentSize();
    }

    private void GenerateMapUI()
    {
        // Clear existing
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        instantiatedNodes.Clear();

        var map = RunManager.Instance.currentMap;

        // 1. Create Nodes
        foreach (var nodeInstance in map.nodes)
        {
            GameObject go = Instantiate(nodePrefab, contentParent);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = nodeInstance.position;

            MapNode nodeComp = go.GetComponent<MapNode>();
            MapNodeData dataAsset = allNodeData.Find(d => d.nodeType == nodeInstance.nodeType);
            
            bool isReachable = IsNodeReachable(nodeInstance, map);
            nodeComp.Setup(nodeInstance, dataAsset, isReachable);
            
            instantiatedNodes.Add(nodeInstance.nodeID, nodeComp);
        }

        // 2. Create Lines
        foreach (var nodeInstance in map.nodes)
        {
            foreach (int childID in nodeInstance.connectedTo)
            {
                CreateLine(nodeInstance.nodeID, childID);
            }
        }
    }

    private bool IsNodeReachable(MapNodeInstance node, MapGraphData map)
    {
        if (node.layer == 0) return true; // First layer is always reachable (starting points)
        if (map.currentNodeID == -1) return false; // Not at first layer and no node selected

        // Check if this node is connected to the current node
        var currentNode = map.nodes.Find(n => n.nodeID == map.currentNodeID);
        return currentNode != null && currentNode.connectedTo.Contains(node.nodeID);
    }

    private void CreateLine(int fromID, int toID)
    {
        if (!instantiatedNodes.ContainsKey(fromID) || !instantiatedNodes.ContainsKey(toID)) return;

        Vector2 fromPos = instantiatedNodes[fromID].GetComponent<RectTransform>().anchoredPosition;
        Vector2 toPos = instantiatedNodes[toID].GetComponent<RectTransform>().anchoredPosition;

        GameObject lineGo = Instantiate(linePrefab, contentParent);
        lineGo.transform.SetAsFirstSibling(); // Lines behind nodes
        
        RectTransform rt = lineGo.GetComponent<RectTransform>();
        Vector2 dir = toPos - fromPos;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        rt.anchoredPosition = fromPos;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, distance);
        rt.localRotation = Quaternion.Euler(0, 0, angle - 90f); // Adjust for 0,0,0 rotation pointing up
    }

    private void AdjustContentSize()
    {
        var map = RunManager.Instance.currentMap;
        float maxY = 0;
        foreach (var node in map.nodes)
        {
            if (node.position.y > maxY) maxY = node.position.y;
        }

        RectTransform contentRT = contentParent.GetComponent<RectTransform>();
        contentRT.sizeDelta = new Vector2(contentRT.sizeDelta.x, maxY + 300f);
    }
}
