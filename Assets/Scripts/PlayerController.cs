using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    private UnitMovement mover;

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

    void Update()
    {
        if (!mover.isMoving && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
            Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

            List<Node> path = pathfinding.FindPath(currentPos, gridPos);
            if (path != null && path.Count > 0)
            {
                StartCoroutine(mover.MoveAlongPath(path));
            }
        }
    }
}
