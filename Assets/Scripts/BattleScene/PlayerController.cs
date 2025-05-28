using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private int movementRange;
    private UnitMovement mover;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinding pathfinding;

    [HideInInspector] public bool hasMoved = false;

    public PlayerMenu playerMenu;

    public int index;

    [SerializeField] private UnitStats stats;

    private TransformGridHelper gridHelper;
    private UnitCombat combat;

    void Awake()
    {
        mover = GetComponent<UnitMovement>();
        gridHelper = GetComponent<TransformGridHelper>();
        combat = GetComponent<UnitCombat>();

        if (gridManager == null) gridManager = FindFirstObjectByType<GridManager>();
        if (pathfinding == null) pathfinding = FindFirstObjectByType<Pathfinding>();
    }

    void Update()
    {
        //Only continue if no other player character was clicked on and hasn't acted yet
        if (playerMenu.index != index && playerMenu.index != -1)
        {
            return;
        }

        if (mover.isMoving || hasMoved) return;

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Converts world position to grid position
        int tileSize = gridManager.TileSize;

        Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(worldPos.x / tileSize), Mathf.RoundToInt(worldPos.y / tileSize));

        // Check if player is clicked on
        if (!playerMenu.gameObject.activeSelf
            && gridHelper.GridPosition == gridPos)
        {
            playerMenu.Setup(index, stats.data.attackPower, stats.data.moveDistance, stats.currentHealth, stats.data.maxHealth);
            return;
        }

        // Check if enemy is clicked on
        UnitStats targetStats = null;
        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            var eHelper = enemy.GetComponent<TransformGridHelper>();
            if (eHelper.GridPosition == gridPos)
            {
                targetStats = enemy.gameObject.GetComponent<UnitStats>();
            }
        }
        if (!playerMenu.gameObject.activeSelf
            && targetStats != null && playerMenu.index == -1)
        {
            playerMenu.EnemySetup(targetStats.currentHealth, targetStats.data.maxHealth);
            return;
        }

        // if menu is up but no action chosen, ignore clicks
        if (playerMenu.gameObject.activeSelf)
            return;

        // --- MOVE MODE ---
        if (playerMenu.Moving)
        {
            TryMove(gridPos);
            return;
        }

        // --- ATTACK MODE ---
        if (playerMenu.Attacking)
        {
            TryAttack(gridPos);
            return;
        }
    }

    private void TryMove(Vector2Int targetGrid)
    {
        Vector2Int startGrid = gridHelper.GridPosition;
        var path = pathfinding.FindPath(startGrid, targetGrid);
        if (path == null || path.Count == 0) return;
        if (path.Count > movementRange)
        {
            Debug.Log($"Too far: need {path.Count}, have {movementRange}");
            return;
        }

        // consume move
        playerMenu.Moving = false;
        playerMenu.index = -1;
        StartCoroutine(MoveAndEndTurn(path));
    }

    private void TryAttack(Vector2Int targetGrid)
    {
        // find any enemy at that grid cell
        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            var eHelper = enemy.GetComponent<TransformGridHelper>();
            if (eHelper.GridPosition == targetGrid)
            {
                combat.TryAttack(enemy.gameObject);
                hasMoved = true;
                playerMenu.Attacking = false;
                playerMenu.index = -1;
                return;
            }
        }
        Debug.Log("No enemy to attack there");
    }

    private IEnumerator MoveAndEndTurn(List<Node> path)
    {
        yield return StartCoroutine(mover.MoveAlongPath(path));
        hasMoved = true;
    }

}
