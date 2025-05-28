using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [Tooltip("Tiles per second (will be multiplied by tileSize)")]
    [SerializeField] private float tilesPerSecond = 3f;

    public bool isMoving { get; private set; }

    private GridManager gridManager;
    private float unitsPerSecond;

    void Awake()
    {
        // Find and cache GridManager
        gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager == null)
            Debug.LogError("UnitMovement: No GridManager found in scene!", this);

        // Compute world‐space speed
        unitsPerSecond = tilesPerSecond * gridManager.TileSize;
    }

    public IEnumerator MoveAlongPath(List<Node> path)
    {
        if (gridManager == null)
            yield break;

        isMoving = true;

        foreach (var node in path)
        {
            Vector2Int nextGrid = node.GridPosition;

            // Free whatever tile you were on before
            // We'll compute your current grid from your position:
            Vector2Int currentGrid = new Vector2Int(
                Mathf.RoundToInt(transform.position.x / gridManager.TileSize),
                Mathf.RoundToInt(transform.position.y / gridManager.TileSize));
            gridManager.SetOccupied(currentGrid, false);

            // Compute world-space target
            Vector3 target = new Vector3(
                nextGrid.x * gridManager.TileSize,
                nextGrid.y * gridManager.TileSize,
                0f
            );

            // Slide towards it
            while ((transform.position - target).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    unitsPerSecond * Time.deltaTime
                );
                yield return null;
            }
            // Snap exactly
            transform.position = target;

            // Mark the new tile occupied
            gridManager.SetOccupied(nextGrid, true);
        }

        isMoving = false;
    }
}
