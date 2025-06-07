using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap ground;
    public Tilemap obstacles;
    private Dictionary<Vector2Int, GameObject> unitMap = new Dictionary<Vector2Int, GameObject>();

    public bool IsOccupied(Vector2Int gridPos)
    {
        return unitMap.ContainsKey(gridPos);
    }

    public void SetGroundTileColor(Vector2Int gridPos, Color color)
    { 
        Vector3Int cell = new Vector3Int(gridPos.x, gridPos.y, 0);

        Debug.Log($"Highlighting tile at {cell}");

        if (!ground.HasTile(cell))
            return;
        ground.SetTileFlags(cell, TileFlags.None);
        Debug.Log(cell.ToString());
        ground.SetColor(cell, color);
    }
    public void ResetGroundTileColor(Vector2Int gridPos)
    {
        Vector3Int cell = new Vector3Int(gridPos.x, gridPos.y, 0);
        if (!ground.HasTile(cell))
            return;

        ground.SetTileFlags(cell, TileFlags.None);
        ground.SetColor(cell, Color.white);
    }
    public void ResetAllGroundTileColors()
    {
        BoundsInt bounds = ground.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                ResetGroundTileColor(new Vector2Int(x, y));
            }
        }
    }

    public Vector2Int GetGridPos(Vector3 worldClick)
    {
        Vector3Int worldPos = ground.WorldToCell(worldClick);
        Vector2Int gridPos = new Vector2Int(worldPos.x, worldPos.y);
        return gridPos;
    }

    public void RegisterUnit(Vector2Int gridPos, GameObject unit)
    {
        unitMap[gridPos] = unit;
    }

    public void UnregisterUnit(Vector2Int gridPos)
    {
        unitMap.Remove(gridPos);
    }

    public GameObject GetUnitAt(Vector2Int gridPos)
    {
        unitMap.TryGetValue(gridPos, out var unit);
        Debug.Log("Value of unit: " + unit);
        return unit;
    }
}