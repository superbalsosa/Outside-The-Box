using UnityEngine;

public class Monitor : BasicEventObject
{
    [SerializeField] private GameObject MonitorCanvasUI;

    private bool isUIOpen = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenClosedUI()
    {
        if (isUIOpen)
        {
            MonitorCanvasUI.SetActive(false);
            isUIOpen = false;
        }
        else
        {
            MonitorCanvasUI.SetActive(true);
            isUIOpen = true;
        }
    }
}
