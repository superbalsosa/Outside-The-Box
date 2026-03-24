using UnityEngine;

[CreateAssetMenu(fileName = "NewNodeData", menuName = "Map/Node Data")]
public class MapNodeData : ScriptableObject
{
    public string nodeName;
    public Sprite nodeIcon;
    public NodeType type;

    public enum NodeType { test, normal }
}