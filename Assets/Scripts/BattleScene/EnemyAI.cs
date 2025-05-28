using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform playerTransform;

    [SerializeField] private int movementRange = 5;
    private UnitMovement mover;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private GameObject parent;
    [SerializeField] private TransformGridHelper gridHelper;

    private int tileSize;

    void Awake()
    {
        mover = GetComponent<UnitMovement>();
        if (gridManager == null) gridManager = FindFirstObjectByType<GridManager>();
        if (pathfinding == null) pathfinding = FindFirstObjectByType<Pathfinding>();
        tileSize = gridManager.TileSize;
    }

    public IEnumerator TakeTurnCoroutine()
    {
        //Find closest player to move towards
        foreach (Transform child in parent.transform)
        {
            playerTransform = findClosest(child);
        }

        if (this == null) yield break;

        Debug.Log("EnemyAI: TakeTurnCoroutine ENTER");

        // Convert world pos → grid coords
        Vector2Int start = Vector2Int.RoundToInt((Vector2)transform.position / tileSize);
        Vector2Int playerGrid = Vector2Int.RoundToInt((Vector2)playerTransform.position / tileSize);
        Debug.Log($"EnemyAI: start={start}, playerAt={playerGrid}");

        // If already adjacent, skip moving
        if (Mathf.Abs(start.x - playerGrid.x) + Mathf.Abs(start.y - playerGrid.y) == 1)
        {
            tryAttack(playerGrid);
            yield break;
        }

        // Examine each of the 4 neighbors around the player
        Vector2Int[] dirs = {
            Vector2Int.up, Vector2Int.right,
            Vector2Int.down, Vector2Int.left
        };

        List<Node> bestPath = null;
        int bestLength = int.MaxValue;

        foreach (var d in dirs)
        {
            Vector2Int target = playerGrid + d;
            // must be in bounds, walkable, and not occupied
            var node = gridManager.GetNode(target);
            if (node == null || !node.IsWalkable || node.IsOccupied)
                continue;

            var path = pathfinding.FindPath(start, target);
            if (path != null && path.Count > 0 && path.Count < bestLength)
            {
                bestPath = path;
                bestLength = path.Count;
            }
        }

        if (bestPath == null)
        {
            Debug.Log("EnemyAI: No reachable adjacent tile to player");
            yield break;
        }

        Debug.Log($"EnemyAI: Best path to adjacent tile has {bestPath.Count} steps");

        // Trim to movementRange
        int steps = Mathf.Min(movementRange, bestPath.Count);
        var limited = bestPath.GetRange(0, steps);

        Debug.Log($"EnemyAI: moving {steps}/{bestPath.Count} tiles");
        yield return StartCoroutine(mover.MoveAlongPath(limited));

        Debug.Log("EnemyAI: MoveAlongPath complete");

        // Optional: if you ended up adjacent, attack
        //Vector2Int endPos = Vector2Int.RoundToInt((Vector2)transform.position / tileSize);
        tryAttack(playerGrid);
    }

    void tryAttack(Vector2Int playerGrid)
    {
        if (Mathf.Abs(gridHelper.GridPosition.x - playerGrid.x) + Mathf.Abs(gridHelper.GridPosition.y - playerGrid.y) == 1)
        {
            Debug.Log("EnemyAI: Adjacent after move, attacking");
            GetComponent<UnitCombat>()?.TryAttack(playerTransform.gameObject);
        }
    }

    Transform findClosest(Transform child)
    {
        Vector2Int start = Vector2Int.RoundToInt((Vector2)transform.position / tileSize);
        Vector2Int playerGrid = Vector2Int.RoundToInt((Vector2)child.position / tileSize);
        double newDistance = Math.Sqrt(Math.Pow((playerGrid.x - start.x), 2) + Math.Pow((playerGrid.y - start.y), 2));

        start = Vector2Int.RoundToInt((Vector2)transform.position / tileSize);
        playerGrid = Vector2Int.RoundToInt((Vector2)playerTransform.position / tileSize);
        double oldDistance = Math.Sqrt(Math.Pow((playerGrid.x - start.x), 2) + Math.Pow((playerGrid.y - start.y), 2));

        if (newDistance > oldDistance)
        {
            return playerTransform;
        }
        return child;

    }
}
