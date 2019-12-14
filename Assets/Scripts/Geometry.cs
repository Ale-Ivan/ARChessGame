using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry
{
    public static Vector3 PointFromGrid(Vector2Int gridPoint)
    {
        float x = -2.1f + gridPoint.y * 0.6f;
        float z = -2.1f + gridPoint.x * 0.6f;
        return new Vector3(x, 0.01f, z);
    }

    public static Vector2Int GridFromPoint(Vector3 point)
    {
        int row = Mathf.FloorToInt((point.z + 2.1f) / 0.6f);
        int col = Mathf.FloorToInt((point.x + 2.1f) / 0.6f);
        return new Vector2Int(row, col);
    }
}
