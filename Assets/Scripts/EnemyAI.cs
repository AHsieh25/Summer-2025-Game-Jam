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

        Debug.Log("EnemyAI: TakeTurnCoroutine ENTER");
        // Compute start/end
        Vector2Int start = Vector2Int.RoundToInt(transform.position / tileSize);
        Vector2Int end = Vector2Int.RoundToInt(playerTransform.position / tileSize);

        List<Node> path = pathfinding.FindPath(start, end);
        if (path == null || path.Count == 0)
        {
            yield break;
        }

        // Limit movement range
        int steps = Mathf.Min(movementRange, path.Count);
        List<Node> limitPath = path.GetRange(0, steps);

        yield return StartCoroutine(mover.MoveAlongPath(limitPath));
        // TO-DO: Add attack logic
    }
}
