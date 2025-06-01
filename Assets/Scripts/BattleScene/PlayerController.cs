using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int movementRange;
    [SerializeField] private int attackRange;
    private UnitMovement mover;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinding pathfinding;

    [HideInInspector] public bool hasMoved = false;
    private bool viewing = false;
    private bool attacking = false;

    public PlayerMenu playerMenu;
    public AttackMenu attackMenu;

    [SerializeField] private UnitStats stats;
    private UnitCombat combat;

    [SerializeField] private CurrentData cd;

    public int index;

    void Awake()
    {
        mover = GetComponent<UnitMovement>();
        combat = GetComponent<UnitCombat>();

        if (gridManager == null) gridManager = FindFirstObjectByType<GridManager>();
        if (pathfinding == null) pathfinding = FindFirstObjectByType<Pathfinding>();
    }

    void Update()
    {
        if (playerMenu.Attacking && playerMenu.index == index)
        {
            if (!attacking)
            {
                cd.currentData.stats = stats;
                attacking = true;
                attackMenu.Setup();
                //ViewAttack();
                gridManager.ResetAllGroundTileColors();
            }

            TryAttack();
            return;
        }

        if (!playerMenu.gameObject.activeSelf && !playerMenu.Moving && !playerMenu.Attacking && viewing)
        {
            gridManager.ResetAllGroundTileColors();
            viewing = false;
        }

        if (playerMenu.index != -1 && playerMenu.index != index)
            return;

        if (mover.isMoving || hasMoved) return;

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePx = Mouse.current.position.ReadValue();
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(mousePx);
        worldClick.z = 0f;
        Vector3Int clickedCell = gridManager.ground.WorldToCell(worldClick);
        Vector2Int clickGrid = new Vector2Int(clickedCell.x, clickedCell.y);

        Vector3Int myCell = gridManager.ground.WorldToCell(transform.position);
        Vector2Int myGrid = new Vector2Int(myCell.x, myCell.y);

        if (!playerMenu.gameObject.activeSelf && clickGrid == myGrid)
        {
            playerMenu.Setup(index, stats.data.attackPower, stats.data.moveDistance, stats.currentHealth, stats.data.maxHealth, stats.weaponName);
            ViewMove();
            viewing = true;
            return;
        }

        UnitStats clickedEnemyStats = null;
        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            Vector3Int enemyCell = gridManager.ground.WorldToCell(enemy.transform.position);
            Vector2Int enemyGrid = new Vector2Int(enemyCell.x, enemyCell.y);
            if (enemyGrid == clickGrid)
            {
                clickedEnemyStats = enemy.GetComponent<UnitStats>();
                break;
            }
        }

        if (!playerMenu.gameObject.activeSelf && clickedEnemyStats != null && playerMenu.index == -1)
        {
            playerMenu.EnemySetup(clickedEnemyStats.currentHealth, clickedEnemyStats.data.maxHealth
            );
            return;
        }

        if (playerMenu.gameObject.activeSelf)
            return;

        if (playerMenu.Moving)
        {
            TryMove(clickGrid);
            return;
        }
    }

    private void TryMove(Vector2Int targetGrid)
    {
        Vector3Int myCell = gridManager.ground.WorldToCell(transform.position);
        Vector2Int startGrid = new Vector2Int(myCell.x, myCell.y);

        List<Vector2Int> path = pathfinding.FindPath(startGrid, targetGrid);
        if (path == null || path.Count == 0)
            return;

        if (path.Count > movementRange)
        {
            Debug.Log($"Move too far: required {path.Count}, max {movementRange}");
            return;
        }

        playerMenu.Moving = false;
        playerMenu.index = -1;
        gridManager.ResetAllGroundTileColors();
        viewing = false;
        StartCoroutine(MoveAndEndTurn(path));
    }

    private void TryAttack()
    {
        if (attackMenu.back)
        {
            attackMenu.back = false;
            playerMenu.Attacking = false;
            playerMenu.gameObject.SetActive(true);
            viewing = false;
            attacking = false;
            ViewMove();
        }

        if (!attackMenu.done) return;

        Vector3Int myCell3D = gridManager.ground.WorldToCell(transform.position);
        Vector2Int unitGrid = new Vector2Int(myCell3D.x, myCell3D.y);
        
        List<Vector2Int> targetCells = new List<Vector2Int>();
        foreach (Vector2Int v in stats.attackGrid)
        {
            Vector2Int targetGrid = Vector2Int.zero;
            if (attackMenu.up)
            {
                targetGrid = v;
            }
            else if (attackMenu.right)
            {
                targetGrid = new Vector2Int(v.y, -v.x);
            }
            else if (attackMenu.down)
            {
                targetGrid = new Vector2Int(-v.x, -v.y);
            }
            else if (attackMenu.left)
            {
                targetGrid = new Vector2Int(-v.y, v.x);
            }

            Vector2Int targetCell = unitGrid + targetGrid;
            Debug.Log("ug: " + unitGrid + "tg: " + targetGrid + " = " + targetCell);
            targetCells.Add(targetCell);
        }
        Debug.Log(targetCells.Count);
        Debug.Log(String.Join("\n", targetCells));

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Vector3Int eCell3D = gridManager.ground.WorldToCell(enemy.transform.position);
            Vector2Int eGrid = new Vector2Int(eCell3D.x, eCell3D.y);

            if (targetCells.Contains(eGrid))
            {
                combat.TryAttack(enemy.gameObject);
                continue;
            }
        }

        hasMoved = true;
        playerMenu.Attacking = false;
        playerMenu.index = -1;
        attacking = false;
        attackMenu.done = false;
    }

    private IEnumerator MoveAndEndTurn(List<Vector2Int> path)
    {
        yield return StartCoroutine(mover.MoveAlongPath(path));
        hasMoved = true;
    }

    private void ViewMove()
    {
        gridManager.ResetAllGroundTileColors();

        Vector3Int myCell = gridManager.ground.WorldToCell(transform.position);
        Vector2Int myGrid = new Vector2Int(myCell.x, myCell.y);

        BoundsInt bounds = gridManager.ground.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);

                Vector3Int cell3 = new Vector3Int(x, y, 0);
                if (!gridManager.ground.HasTile(cell3))
                    continue;

                List<Vector2Int> path = pathfinding.FindPath(myGrid, cell);
                if (path == null || path.Count == 0)
                    continue;

                if (path.Count <= movementRange)
                {
                    gridManager.SetGroundTileColor(cell, new Color(0f, 0f, 1f));
                }
            }
        }
        viewing = true;
    }

    private void ViewAttack()
    {
        gridManager.ResetAllGroundTileColors();

        Vector3Int myCell3D = gridManager.ground.WorldToCell(transform.position);
        Vector2Int unitGrid = new Vector2Int(myCell3D.x, myCell3D.y);

        List<Vector2Int> targetCells = new List<Vector2Int>();
        Vector2Int targetGrid = Vector2Int.zero;
        foreach (Vector2Int v in stats.attackGrid)
        {
            targetGrid = v;
            gridManager.SetGroundTileColor(unitGrid + targetGrid, new Color(1f, 0f, 0f));
            targetGrid = new Vector2Int(v.y, -v.x);
            gridManager.SetGroundTileColor(unitGrid + targetGrid, new Color(1f, 0f, 0f));
            targetGrid = new Vector2Int(-v.x, -v.y);
            gridManager.SetGroundTileColor(unitGrid + targetGrid, new Color(1f, 0f, 0f));
            targetGrid = new Vector2Int(-v.y, v.x);
            gridManager.SetGroundTileColor(unitGrid + targetGrid, new Color(1f, 0f, 0f));
        }
    }
}
