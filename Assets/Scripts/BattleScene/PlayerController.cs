using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private int movementRange;
    private UnitMovement mover;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinding pathfinding;

    [HideInInspector] public bool hasMoved = false;
    public bool viewing = false;
    public bool attacking = false;

    public PlayerMenu playerMenu;
    public AttackMenu attackMenu;

    public int index;

    [SerializeField] private UnitStats stats;

    private TransformGridHelper gridHelper;
    private UnitCombat combat;

    private SaveData sd;
    [SerializeField] private CurrentData cd;

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
        if (playerMenu.Attacking && playerMenu.index == index)
        {
            if (!attacking)
            {
                attacking = true;
                attackMenu.Setup();
                cd.currentData.stats = stats;
                cd.currentData.helper = gridHelper;
                viewMove(false);
            }
            TryAttack();
            return;
        }

            //Only continue if no other player character was clicked on and hasn't acted yet
            if (!playerMenu.gameObject.activeSelf && !playerMenu.Moving &&!playerMenu.Attacking && viewing)
            viewMove(false);

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
            playerMenu.Setup(index, stats.data.attackPower, stats.data.moveDistance, stats.currentHealth, stats.data.maxHealth, stats.weaponName);
            viewMove(true);
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
        /*
        if (playerMenu.Attacking)
        {
            TryAttack(gridPos);
            return;
        }
        */
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
        viewMove(false);
        StartCoroutine(MoveAndEndTurn(path));
    }

    private void TryAttack()
    {
        if (attackMenu.back)
        {
            attackMenu.back = false;
            playerMenu.Attacking = false;
            attacking = false;
            playerMenu.gameObject.SetActive(true);
            viewMove(true);
        }

        if (!attackMenu.done)
        {
            return;
        }

        Vector2Int targetGrid = new Vector2Int(0, 0);
        foreach (Vector2Int v in stats.data.attackGrid)
        {
            if (attackMenu.up)
            {
                targetGrid = gridHelper.GridPosition + v;
            }
            else if (attackMenu.right)
            {
                targetGrid = v;
                int temp = targetGrid.x;
                targetGrid.x = targetGrid.y;
                targetGrid.y = temp;
                targetGrid = gridHelper.GridPosition + targetGrid;
            }
            else if (attackMenu.down)
            {
                targetGrid = v;
                targetGrid.y = targetGrid.y * -1;
                targetGrid = gridHelper.GridPosition + targetGrid;
            }
            else if (attackMenu.left)
            {
                targetGrid = v;
                int temp = targetGrid.x;
                targetGrid.x = targetGrid.y;
                targetGrid.y = temp;
                targetGrid.x = targetGrid.x * -1;
                targetGrid = gridHelper.GridPosition + targetGrid;
            }

            // find any enemy at that grid cell
            foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
            {
                var eHelper = enemy.GetComponent<TransformGridHelper>();
                if (eHelper.GridPosition == targetGrid)
                {
                    combat.TryAttack(enemy.gameObject);
                    continue;
                }
            }
            //Debug.Log("No enemy to attack there");
        }
        hasMoved = true;
        playerMenu.Attacking = false;
        playerMenu.index = -1;
        attacking = false;
        attackMenu.done = false;
    }

    private IEnumerator MoveAndEndTurn(List<Node> path)
    {
        yield return StartCoroutine(mover.MoveAlongPath(path));
        hasMoved = true;
    }

    private void viewMove(bool view)
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                var path = pathfinding.FindPath(gridHelper.GridPosition, new Vector2Int(i, j));
                if (path == null || path.Count == 0) continue;
                if (path.Count <= movementRange)
                {
                    gridManager.SetColor(new Vector2Int(i, j), view);
                }
            }
        }
        //viewAttack(view);
        viewing = view;
    }

    private void viewAttack(bool view)
    {
        Vector2Int targetGrid;
        foreach (Vector2Int v in stats.data.attackGrid)
        {
            targetGrid = gridHelper.GridPosition + v;
            gridManager.SetColor2(targetGrid, view);

            targetGrid = v;
            int temp = targetGrid.x;
            targetGrid.x = targetGrid.y;
            targetGrid.y = temp;
            targetGrid = gridHelper.GridPosition + targetGrid;
            gridManager.SetColor2(targetGrid, view);

            targetGrid = v;
            targetGrid.y = targetGrid.y * -1;
            targetGrid = gridHelper.GridPosition + targetGrid;
            gridManager.SetColor2(targetGrid, view);

            targetGrid = v;
            temp = targetGrid.x;
            targetGrid.x = targetGrid.y;
            targetGrid.y = temp;
            targetGrid.x = targetGrid.x * -1;
            targetGrid = gridHelper.GridPosition + targetGrid;
            gridManager.SetColor2(targetGrid, view);
        }
    }
}
