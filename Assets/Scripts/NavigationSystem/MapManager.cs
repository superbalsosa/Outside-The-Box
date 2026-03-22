using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapGenerator generator;
    public MapDisplay display;

    private MapNode currentNode;
    private int currentLayerIndex = -1;
    private List<MapLayer> mapLayers;

    private void Start()
    {
        List<MapLayer> layers = generator.GenerateMap();

        display.DrawMap(layers, this);

        UpdateNodeStates();
    }
    public void SelectNode(MapNode node, MapNodeView view)
    {
        int nodeLayer = generator.GetLayerIndex(node);

        bool isFirstLayer = currentLayerIndex == -1 && nodeLayer == 0;
        bool isChild = currentNode != null && currentNode.children.Contains(node);

        if (isFirstLayer || isChild)
        {
            currentNode = node;
            currentLayerIndex = nodeLayer;

            Debug.Log("Enter to node: " + node.position);

            UpdateNodeStates();
        }
    }
    private void UpdateNodeStates()
    {
        MapNodeView[] allViews = FindObjectsOfType<MapNodeView>();

        foreach (var view in allViews)
        {
            MapNode node = view.GetNodeData();
            int nodeLayer = generator.GetLayerIndex(node);

            bool reachable = false;

            if (currentLayerIndex == -1)
            {
                reachable = nodeLayer == 0;
            } 
            else
            {
                reachable = currentNode.children.Contains(node);
            }

            bool visited = node == currentNode;
            view.SetState(reachable, visited);
        }
    }
}