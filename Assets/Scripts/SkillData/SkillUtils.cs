using System.Collections.Generic;
using UnityEngine;

public enum Direction { Up, Right, Down, Left }

public static class SkillUtils
{
    public static List<Vector2Int> RotateOffsets(List<Vector2Int> baseOffsets, Direction dir)
    {
        var result = new List<Vector2Int>(baseOffsets.Count);
        foreach (var v in baseOffsets)
        {
            switch (dir)
            {
                case Direction.Up:
                    result.Add(new Vector2Int(v.x, v.y));
                    break;
                case Direction.Right:
                    result.Add(new Vector2Int(v.y, -v.x));
                    break;
                case Direction.Down:
                    result.Add(new Vector2Int(-v.x, -v.y));
                    break;
                case Direction.Left:
                    result.Add(new Vector2Int(-v.y, v.x));
                    break;
            }
        }
        return result;
    }
}
