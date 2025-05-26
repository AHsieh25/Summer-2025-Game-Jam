using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private int movementRange;
    private UnitMovement mover;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinding pathfinding;

    [HideInInspector] public bool hasMoved = false;

    public PlayerMenu PlayerMenu;

    void Awake()
    {
        mover = GetComponent<UnitMovement>();
        if (gridManager == null) gridManager = FindFirstObjectByType<GridManager>();
        if (pathfinding == null) pathfinding = FindFirstObjectByType<Pathfinding>();
    }

    void Update()
    {
        if (mover.isMoving || hasMoved) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // Converts world position to grid position
            int tileSize = gridManager.TileSize;

            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(worldPos.x / tileSize), Mathf.RoundToInt(worldPos.y / tileSize));
            Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(transform.position.x / tileSize), Mathf.RoundToInt(transform.position.y / tileSize));

            List<Node> path = pathfinding.FindPath(currentPos, gridPos);

            if (path == null) return;

            if (path.Count == 0 && !PlayerMenu.CheckVisible())
            {
                PlayerMenu.Setup();
                return;
            }

            if (!PlayerMenu.CheckMoving())
            {
                return;
            }

            if (path.Count > movementRange)
            {
                Debug.Log($"Target too far: range {movementRange}, need {path.Count}.");
                return;
            }

            PlayerMenu.ChangeMoving();
            StartCoroutine(MoveAndSignal(path));
        }
    }

    private IEnumerator MoveAndSignal(List<Node> path)
    {
        yield return StartCoroutine(mover.MoveAlongPath(path));
        hasMoved = true;
    }
}
