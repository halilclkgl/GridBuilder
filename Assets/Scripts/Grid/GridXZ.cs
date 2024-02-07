using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridXZ 
{

    private int width; // Grid'in geni�li�i
    private int height; // Grid'in y�ksekli�i
    private float cellSize; // Her h�crenin boyutu
    private Vector3 originPosition; // Grid'in ba�lang�� noktas� (sol alt k��e)

    private GridObject[,] gridArray; // Grid'i temsil eden 2 boyutlu dizi

    public GridXZ(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        // Grid'i olu�tur
        gridArray = new GridObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridArray[x, z] = new GridObject(x, z);
            }
        }
    }

    // Grid'in geni�li�ini d�nd�r�r
    public int GetWidth()
    {
        return width;
    }

    // Grid'in y�ksekli�ini d�nd�r�r
    public int GetHeight()
    {
        return height;
    }

    // Her h�crenin boyutunu d�nd�r�r
    public float GetCellSize()
    {
        return cellSize;
    }

    // Grid'in ba�lang�� noktas�n� d�nd�r�r
    public Vector3 GetOriginPosition()
    {
        return originPosition;
    }

    // Grid'de belirtilen konumda bir h�cre al�r
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

    // Belirtilen koordinatlar�n grid i�inde olup olmad���n� kontrol eder
    public bool IsInsideGrid(int x, int z)
    {
        return x >= 0 && x < width && z >= 0 && z < height;
    }
    public Vector3 GetWorldPosition(int x, int z)
    {
        return originPosition + new Vector3(x * cellSize, 0, z * cellSize);
    }

    // H�crenin doluluk durumunu kontrol eder
    public bool IsCellOccupied(int x, int z)
    {
        if (IsInsideGrid(x, z))
        {
            GridObject cellObject = gridArray[x, z];
            return cellObject.IsOccupied();
        }
        else
        {
            // Ge�ersiz h�crelerin her zaman bo� oldu�unu varsay�yoruz
            return false;
        }
    }
}
