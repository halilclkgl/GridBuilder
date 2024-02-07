using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    private GridXZ grid;


    private void Awake()
    {
        int gridWidth = 20;
        int gridHeight = 20;
        float cellSize = 10f;
        grid = new GridXZ(gridWidth, gridHeight, cellSize, Vector3.zero);

    }

    public GridXZ ReturnGrid() { return grid; }
}