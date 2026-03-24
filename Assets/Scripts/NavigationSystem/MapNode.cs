using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapNode
{
    public Vector2 position;
    public MapNodeData data;
    public List<MapNode> children = new List<MapNode>();
    public bool isReachable;
}

[Serializable]
public class MapLayer
{
    public List<MapNode> nodes = new List<MapNode>();
}
