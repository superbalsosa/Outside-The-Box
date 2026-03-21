using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int totalLayers = 15;
    public int nodesPerLayer = 5;
    public float xSpacing = 150f;
    public float ySpacing = 200f;

    private List<MapLayer> _mapLayers = new List<MapLayer>();

    public void GenerateMap()
    {
        _mapLayers.Clear();

        for (int i = 0; i < totalLayers; i++)
        {
            MapLayer layer = new MapLayer();
            int count = Random.Range(3, nodesPerLayer + 1);

            for (int j = 0; j < count; j++)
            {
                MapNode node = new MapNode();
                node.position = new Vector2(j * xSpacing, i * ySpacing);
                layer.nodes.Add(node);
            }
            _mapLayers.Add(layer);
        }

        for (int i = 0; i < totalLayers -1 ; i++)
        {
            foreach (var node in _mapLayers[i].nodes)
            {
                int randomIndex = Random.Range(0, _mapLayers[i + 1].nodes.Count);
                node.children.Add(_mapLayers[i + 1].nodes[randomIndex]);
            }
        }
    }
}
