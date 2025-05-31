using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{
    public GameObject parent;

    [SerializeField] private int movementRange = 5;
    private UnitMovement mover;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinding pathfinding;

    private Tilemap groundTilemap;
    private Tilemap obstacleTilemap;

    void Awake()
    {
        mover = GetComponent<UnitMovement>();

        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        groundTilemap = gridManager.ground;
        obstacleTilemap = gridManager.obstacles;

        if (pathfinding == null)
            pathfinding = FindFirstObjectByType<Pathfinding>();
    }

    public IEnumerator TakeTurnCoroutine()
    {
        Transform closestPlayer = null;
        float bestDistSqr = float.MaxValue;

        Vector3Int myCell = groundTilemap.WorldToCell(transform.position);
        Vector2Int startGrid = new Vector2Int(myCell.x, myCell.y);

        foreach (Transform child in parent.transform)
        {
            if (child == null) continue;

            Vector3Int pCell = groundTilemap.WorldToCell(child.position);
            Vector2Int pGrid = new Vector2Int(pCell.x, pCell.y);

            float dx = pGrid.x - startGrid.x;
            float dy = pGrid.y - startGrid.y;
            float distSqr = dx * dx + dy * dy;

            if (distSqr < bestDistSqr)
            {
                bestDistSqr = distSqr;
                closestPlayer = child;
            }
        }

        if (closestPlayer == null)
            yield break;

        Vector3Int playerCell = groundTilemap.WorldToCell(closestPlayer.position);
        Vector2Int playerGrid = new Vector2Int(playerCell.x, playerCell.y);

        if (Mathf.Abs(startGrid.x - playerGrid.x) + Mathf.Abs(startGrid.y - playerGrid.y) == 1)
        {
            TryAttack(playerGrid);
            yield break;
        }

        Vector2Int[] dirs = {
            Vector2Int.up, Vector2Int.right,
            Vector2Int.down, Vector2Int.left
        };

        List<Vector2Int> bestPath = null;
        int bestLength = int.MaxValue;

        foreach (var d in dirs)
        {
            Vector2Int target = playerGrid + d;

            Vector3Int tCell = new Vector3Int(target.x, target.y, 0);
            if (!groundTilemap.HasTile(tCell))
                continue;

            if (obstacleTilemap.HasTile(tCell))
                continue;

            if (gridManager.IsOccupied(target))
                continue;

            List<Vector2Int> path = pathfinding.FindPath(startGrid, target);
            if (path != null && path.Count > 0 && path.Count < bestLength)
            {
                bestPath = path;
                bestLength = path.Count;
            }
        }

        if (bestPath == null)
        {
            yield break;
        }

        int steps = Mathf.Min(movementRange, bestPath.Count);
        List<Vector2Int> limitedPath = bestPath.GetRange(0, steps);

        yield return StartCoroutine(mover.MoveAlongPath(limitedPath));

        Vector3Int newCell = groundTilemap.WorldToCell(transform.position);
        Vector2Int newGrid = new Vector2Int(newCell.x, newCell.y);

        if (Mathf.Abs(newGrid.x - playerGrid.x) + Mathf.Abs(newGrid.y - playerGrid.y) == 1)
        {
            TryAttack(playerGrid);
        }
    }

    private void TryAttack(Vector2Int playerGrid)
    {
        Vector3Int myCell = groundTilemap.WorldToCell(transform.position);
        Vector2Int myGrid = new Vector2Int(myCell.x, myCell.y);
        if (Mathf.Abs(myGrid.x - playerGrid.x) + Mathf.Abs(myGrid.y - playerGrid.y) == 1)
        {
            GetComponent<UnitCombat>()?.TryAttack(FindClosestPlayerGameObject().gameObject);
        }
    }

    private Transform FindClosestPlayerGameObject()
    {
        Transform closest = null;
        float bestDistSqr = float.MaxValue;

        Vector3Int myCell = groundTilemap.WorldToCell(transform.position);
        Vector2Int startGrid = new Vector2Int(myCell.x, myCell.y);

        foreach (Transform child in parent.transform)
        {
            if (child == null) continue;

            Vector3Int pCell = groundTilemap.WorldToCell(child.position);
            Vector2Int pGrid = new Vector2Int(pCell.x, pCell.y);

            float dx = pGrid.x - startGrid.x;
            float dy = pGrid.y - startGrid.y;
            float distSqr = dx * dx + dy * dy;

            if (distSqr < bestDistSqr)
            {
                bestDistSqr = distSqr;
                closest = child;
            }
        }

        return closest;
    }
}
