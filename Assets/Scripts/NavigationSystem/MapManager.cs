using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapNode currentNode;

    public void OnNodeClicked(MapNode selectedNode)
    {
        if (currentNode.children.Contains(selectedNode))
        {
            currentNode = selectedNode;
            EnterRoom(selectedNode.data);
            //UpdateVisuals();
        }
    }
    private void EnterRoom(MapNodeData data)
    {
        Debug.Log("Enter to: " + data.nodeName);
    }
}
