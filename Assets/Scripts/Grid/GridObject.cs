using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    // Hücrenin x ve z koordinatlarý
    public int x;
    public int z;
    public int visited = -1;
    // Hücrenin doluluk durumu
    private bool isOccupied;

    public GridObject(int x, int z)
    {
        this.x = x;
        this.z = z;
        this.isOccupied = false; // Baþlangýçta hücre boþ
    }

    public int GetX()
    {
        return x;
    }

    public int GetZ()
    {
        return z;
    }

    // Hücrenin dünya koordinatlarýný döndürür
    public Vector3 GetWorldPosition(float cellSize, Vector3 originPosition)
    {
        return new Vector3(x * cellSize, 0, z * cellSize) + originPosition;
    }

    // Hücrenin doluluk durumunu belirler
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }

    // Hücrenin doluluk durumunu döndürür
    public bool IsOccupied()
    {
        return isOccupied;
    }
}
