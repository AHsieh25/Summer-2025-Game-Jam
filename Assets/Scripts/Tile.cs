using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int GridPosition;

    public void Init(Vector2Int pos)
    {
        GridPosition = pos;
    }
}