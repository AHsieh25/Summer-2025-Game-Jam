using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float tilesPerSecond = 3f;

    public bool isMoving { get; private set; } = false;

    private GridManager gridManager;
    private Tilemap ground;
    private float unitsPerSecond;

    void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        ground = gridManager.ground;
        float cellSize = ground.cellSize.x;
        unitsPerSecond = tilesPerSecond * cellSize;
    }

    void Start()
    {
        Vector3Int startCell = ground.WorldToCell(transform.position);
        Vector2Int startGrid = new Vector2Int(startCell.x, startCell.y);
        gridManager.SetOccupied(startGrid, true);
    }

    public IEnumerator MoveAlongPath(List<Vector2Int> path)
    {
        if (gridManager == null || ground == null || path == null || path.Count == 0)
            yield break;

        isMoving = true;

        foreach (Vector2Int gridPos in path)
        {
            Vector3Int currCell = ground.WorldToCell(transform.position);
            Vector2Int currGrid = new Vector2Int(currCell.x, currCell.y);
            gridManager.SetOccupied(currGrid, false);

            Vector3Int nextCell = new Vector3Int(gridPos.x, gridPos.y, 0);
            Vector3 worldTarget = ground.GetCellCenterWorld(nextCell);

            while ((transform.position - worldTarget).sqrMagnitude > 0.001f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    worldTarget,
                    unitsPerSecond * Time.deltaTime
                );
                yield return null;
            }

            transform.position = worldTarget;

            gridManager.SetOccupied(gridPos, true);
        }
        isMoving = false;
    }
}
