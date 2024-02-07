using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public GridXZ grid;
    public GridBuildingSystem buildingSystem;

    public int width = 10; 
    public int height = 10;
    public float cellSize = 1f; // Hücre boyutu
    public Color lineColor = Color.white; 
    public List<GameObject> spheres = new List<GameObject>();
    private void Awake()
    {
        grid = buildingSystem.ReturnGrid();
        PlaceSpheresInGrid(grid);
    }

    private void Update()
    {
        PlaceSpheresInGrid(grid);
    }
    private void OnDrawGizmos()
    {
        DrawGrid();

    }
    public void PlaceSpheresInGrid(GridXZ grid)
    {
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                Vector3 worldPosition = grid.GetWorldPosition(x, z);
                bool isOccupied = grid.IsCellOccupied(x, z);

                if (!isOccupied && spheres.Count <= x * grid.GetHeight() + z)
                {
                    GameObject redSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    redSphere.transform.position = worldPosition;
                    redSphere.GetComponent<Renderer>().material.color = Color.green; 

                    spheres.Add(redSphere);
                }
                
                else if (isOccupied && spheres.Count > x * grid.GetHeight() + z)
                {
                    spheres[x * grid.GetHeight() + z].GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
    }
    private void DrawGrid()
    {
        Vector3 origin = transform.position;
        origin.x -= width * cellSize * 0.5f;
        origin.z -= height * cellSize * 0.5f;

        for (int i = 0; i <= width; i++)
        {
            Vector3 startPos = origin + Vector3.right * i * cellSize;
            Vector3 endPos = startPos + Vector3.forward * height * cellSize;
            Gizmos.color = lineColor;
            Gizmos.DrawLine(startPos, endPos);
        }

        for (int j = 0; j <= height; j++)
        {
            Vector3 startPos = origin + Vector3.forward * j * cellSize;
            Vector3 endPos = startPos + Vector3.right * width * cellSize;
            Gizmos.color = lineColor;
            Gizmos.DrawLine(startPos, endPos);
        }
    }
}
