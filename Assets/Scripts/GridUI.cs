using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GridUI : MonoBehaviour
{
    public int width = 5;
    public int height = 5;
    public float cellSize = 1f;
    public float gap = 0.1f;
    // public List<GameObject> nodes = new List<GameObject>();

    // Button function to arrange nodes in grid
    [ContextMenu("Arrange Nodes in Grid")]
    public void ArrangeNodesInGrid()
    {
        int nodeCount = transform.childCount;
        if (nodeCount == 0)
        {
            Debug.LogWarning("No nodes to arrange!");
            return;
        }

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                int index = (j * width + i);
                if (index >= nodeCount) return;
                Vector3 position = new Vector3(i * (cellSize + gap), -j * (cellSize + gap), 0);
                transform.GetChild(index).transform.position = position + transform.position;
            }
        }
    }
}

// Custom Editor for the GridUI to add a button in Inspector
[CustomEditor(typeof(GridUI))]
public class GridUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridUI gridManager = (GridUI)target;
        if (GUILayout.Button("Arrange Nodes in Grid"))
        {
            gridManager.ArrangeNodesInGrid();
        }
    }
}