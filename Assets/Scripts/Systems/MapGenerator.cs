using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    private const int MIN_NODES_PER_LAYER = 3;
    private const int MAX_NODES_PER_LAYER = 5;
    private const float X_SPACING = 200f;
    private const float Y_SPACING = 250f;

    public static MapGraphData GenerateMap(int minLayers, int maxLayers)
    {
        MapGraphData graph = new MapGraphData();
        int layers = Random.Range(minLayers, maxLayers + 1);
        List<List<MapNodeInstance>> layerNodes = new List<List<MapNodeInstance>>();

        int nodeIdCounter = 0;

        for (int i = 0; i < layers; i++)
        {
            List<MapNodeInstance> currentLayer = new List<MapNodeInstance>();
            int nodesInThisLayer = (i == 0 || i == layers - 1) ? 1 : Random.Range(MIN_NODES_PER_LAYER, MAX_NODES_PER_LAYER + 1);

            float totalWidth = (nodesInThisLayer - 1) * X_SPACING;
            float startX = -totalWidth / 2f;

            for (int j = 0; j < nodesInThisLayer; j++)
            {
                MapNodeInstance node = new MapNodeInstance();
                node.nodeID = nodeIdCounter++;
                node.layer = i;
                node.position = new Vector2(startX + j * X_SPACING + Random.Range(-20f, 20f), i * Y_SPACING + Random.Range(-20f, 20f));
                node.nodeType = GetRandomNodeType(i, layers);
                
                currentLayer.Add(node);
                graph.nodes.Add(node);
            }
            layerNodes.Add(currentLayer);
        }

        // Connect nodes
        for (int i = 0; i < layers - 1; i++)
        {
            List<MapNodeInstance> currentLayer = layerNodes[i];
            List<MapNodeInstance> nextLayer = layerNodes[i + 1];

            foreach (var node in currentLayer)
            {
                // Each node connects to at least one node in next layer
                int nextIndex = Random.Range(0, nextLayer.Count);
                node.connectedTo.Add(nextLayer[nextIndex].nodeID);

                // Potentially connect to neighbors in next layer
                if (nextLayer.Count > 1)
                {
                    if (Random.value < 0.3f)
                    {
                        int secondIndex = (nextIndex + 1) % nextLayer.Count;
                        if (!node.connectedTo.Contains(nextLayer[secondIndex].nodeID))
                            node.connectedTo.Add(nextLayer[secondIndex].nodeID);
                    }
                }
            }

            // Ensure every node in next layer has at least one incoming connection
            foreach (var nextNode in nextLayer)
            {
                bool hasIncoming = false;
                foreach (var node in currentLayer)
                {
                    if (node.connectedTo.Contains(nextNode.nodeID))
                    {
                        hasIncoming = true;
                        break;
                    }
                }

                if (!hasIncoming)
                {
                    int parentIndex = Random.Range(0, currentLayer.Count);
                    currentLayer[parentIndex].connectedTo.Add(nextNode.nodeID);
                }
            }
        }

        return graph;
    }

    private static MapNodeData.NodeType GetRandomNodeType(int layer, int totalLayers)
    {
        if (layer == 0) return MapNodeData.NodeType.Battle; // Start with Battle
        if (layer == totalLayers - 1) return MapNodeData.NodeType.Boss;

        float rand = Random.value;
        if (rand < 0.5f) return MapNodeData.NodeType.Battle;
        if (rand < 0.7f) return MapNodeData.NodeType.Event;
        if (rand < 0.85f) return MapNodeData.NodeType.Elite;
        if (rand < 0.95f) return MapNodeData.NodeType.Store;
        return MapNodeData.NodeType.Rest;
    }
}
