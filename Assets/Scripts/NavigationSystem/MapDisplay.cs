using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject linePrefab;
    public RectTransform contentParent;

    public void DrawMap(List<MapLayer> layers, MapManager manager) 
    { 
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        Dictionary<MapNode, Vector2> nodePositions = new Dictionary<MapNode, Vector2>();

        foreach (var layer in layers)
        {
            foreach(var node in layer.nodes)
            {
                GameObject n = Instantiate(nodePrefab, contentParent);
                RectTransform rt = n.GetComponent<RectTransform>();
                rt.anchoredPosition = node.position;

                n.GetComponent<MapNodeView>().Setup(node, manager);
                nodePositions.Add(node, node.position);
            }
        }

        foreach (var layer in layers)
        {
            foreach (var node in layer.nodes)
            {
                foreach (var child in node.children)
                {
                    CreateLine(node.position, child.position);
                }
            }
        }
    }
    private void CreateLine (Vector2 from, Vector2 to)
    {
        GameObject line = Instantiate(linePrefab, contentParent);
        line.transform.SetAsFirstSibling();

        RectTransform rt = line.GetComponent<RectTransform>();
        Vector2 dir = to - from;
        rt.anchoredPosition = from + dir * 0.5f;
        rt.sizeDelta = new Vector2(dir.magnitude, 5f);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }
}