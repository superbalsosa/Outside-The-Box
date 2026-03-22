using UnityEngine;
using UnityEngine.UI;

public class MapNodeView : MonoBehaviour
{
    public Image iconImage;
    public Button button;

    private MapNode nodeData;
    private MapManager manager;

    public void Setup(MapNode _node, MapManager _manager)
    {
        nodeData = _node;
        manager = _manager;

        if (_node.data != null)
        {
            iconImage.sprite = _node.data.nodeIcon;
        }

        button.onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        manager.SelectNode(nodeData, this);
    }
    public void SetState(bool reachable, bool alreadyVisited)
    {
        button.interactable = reachable;
        iconImage.color = reachable ? Color.white : new Color(1, 1, 1, 0.3f);
        if (alreadyVisited)
        {
            iconImage.color = Color.gray;
        }
    }
    public MapNode GetNodeData()
    {
        return nodeData;
    }
}