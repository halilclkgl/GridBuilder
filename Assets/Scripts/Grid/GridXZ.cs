using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridXZ 
{

    private int width; // Grid'in geniþliði
    private int height; // Grid'in yüksekliði
    private float cellSize; // Her hücrenin boyutu
    private Vector3 originPosition; // Grid'in baþlangýç noktasý (sol alt köþe)

    private GridObject[,] gridArray; // Grid'i temsil eden 2 boyutlu dizi

    public GridXZ(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        // Grid'i oluþtur
        gridArray = new GridObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridArray[x, z] = new GridObject(x, z);
            }
        }
    }

    // Grid'in geniþliðini döndürür
    public int GetWidth()
    {
        return width;
    }

    // Grid'in yüksekliðini döndürür
    public int GetHeight()
    {
        return height;
    }

    // Her hücrenin boyutunu döndürür
    public float GetCellSize()
    {
        return cellSize;
    }

    // Grid'in baþlangýç noktasýný döndürür
    public Vector3 GetOriginPosition()
    {
        return originPosition;
    }

    // Grid'de belirtilen konumda bir hücre alýr
    public GridObject GetGridObject(int x, int z)
    {
        if (IsInsideGrid(x, z))
        {
            return gridArray[x, z];
        }
        else
        {
            return null;
        }
    }

    // Belirtilen koordinatlarýn grid içinde olup olmadýðýný kontrol eder
    public bool IsInsideGrid(int x, int z)
    {
        return x >= 0 && x < width && z >= 0 && z < height;
    }
    public Vector3 GetWorldPosition(int x, int z)
    {
        return originPosition + new Vector3(x * cellSize, 0, z * cellSize);
    }

    // Hücrenin doluluk durumunu kontrol eder
    public bool IsCellOccupied(int x, int z)
    {
        if (IsInsideGrid(x, z))
        {
            GridObject cellObject = gridArray[x, z];
            return cellObject.IsOccupied();
        }
        else
        {
            // Geçersiz hücrelerin her zaman boþ olduðunu varsayýyoruz
            return false;
        }
    }
}
