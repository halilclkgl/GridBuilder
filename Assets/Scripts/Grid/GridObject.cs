using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    // H�crenin x ve z koordinatlar�
    public int x;
    public int z;
    public int visited = -1;
    // H�crenin doluluk durumu
    private bool isOccupied;

    public GridObject(int x, int z)
    {
        this.x = x;
        this.z = z;
        this.isOccupied = false; // Ba�lang��ta h�cre bo�
    }

    public int GetX()
    {
        return x;
    }

    public int GetZ()
    {
        return z;
    }

    // H�crenin d�nya koordinatlar�n� d�nd�r�r
    public Vector3 GetWorldPosition(float cellSize, Vector3 originPosition)
    {
        return new Vector3(x * cellSize, 0, z * cellSize) + originPosition;
    }

    // H�crenin doluluk durumunu belirler
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }

    // H�crenin doluluk durumunu d�nd�r�r
    public bool IsOccupied()
    {
        return isOccupied;
    }
}
