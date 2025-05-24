using UnityEngine;

public class Node
{
    public Vector2 GridPosition;
    public bool IsWalkable;
    public int GCost;
    public int HCost;
    public Node Parent;

    public int FCost => GCost + HCost;

    public Node(Vector2Int pos, bool isWalkable)
    {
        GridPosition = pos;
        IsWalkable = isWalkable;
    }
}
