using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry
{
    public static Vector3 PointFromGrid(Vector2Int gridPoint)
    {
        float x = -1.05f + gridPoint.y * 0.3f;
        float z = -1.05f + gridPoint.x * 0.3f;

        //float x = -2.1f + gridPoint.y * 0.6f;
        //float z = -2.1f + gridPoint.x * 0.6f;
        return new Vector3(x, 0.01f, z);
    }

    public static Vector2Int GridFromPoint(Vector3 point)
    {
        double x = System.Math.Round(point.x, 2);
        double z = System.Math.Round(point.z, 2);

        int row = (int)System.Math.Round((z + 1.05) / 0.3);
        int col = (int)System.Math.Round((x + 1.05) / 0.3);

        //int row = (int)System.Math.Round((z + 2.1) / 0.6);
        //int col = (int)System.Math.Round((x + 2.1) / 0.6);
        return new Vector2Int(row, col);
    }
}
