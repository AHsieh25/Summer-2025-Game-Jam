using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    private UnitMovement mover;
    public Transform playerTransform;

    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinding pathfinding;

    void Awake()
    {
        mover = GetComponent<UnitMovement>();

        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
        }

        if (pathfinding == null)
        {
            pathfinding = FindFirstObjectByType<Pathfinding>();
        }
    }

    public void TakeTurn()
    {
        Vector2Int start = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        Vector2Int end = new Vector2Int(Mathf.RoundToInt(playerTransform.position.x), Mathf.RoundToInt(playerTransform.position.y));

        List<Node> path = pathfinding.FindPath(start, end);
        if (path != null && path.Count > 0)
        {
            StartCoroutine(mover.MoveAlongPath(path));
        }
    }
}
