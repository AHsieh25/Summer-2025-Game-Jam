using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private CurrentData cd;

    [HideInInspector] public bool hasMoved = false;

    public PlayerMenu playerMenu;
    public AttackMenu attackMenu;
    public SkillMenu skillMenu;

    private StatsUpdater stats;
    private CharacterStateMachine stateMachine;
    private bool viewing = false;
    private bool attacking = false;
    public int index;

    void Awake() 
    {
        stateMachine = GetComponent<CharacterStateMachine>();
        stats = GetComponent<StatsUpdater>();
    }

    void Update()
    {
        // skill mode
        if (playerMenu.UsingSkill && playerMenu.index == index)
        {
            // First frame entering skill mode?
            if (!skillMenu.UsingSkill)
            {
                // Give the menu your stats so it can list the skills
                skillMenu.Setup(stats);
                gridManager.ResetAllGroundTileColors();
            }

            // If they hit “Back,” bail out and re-open the main menu
            if (skillMenu.back)
            {
                skillMenu.back = false;
                playerMenu.UsingSkill = false;
                playerMenu.gameObject.SetActive(true);
                viewing = false;
                return;
            }

            // If they haven’t chosen yet, just wait here
            if (!skillMenu.done)
                return;

            // They clicked a skill button → perform it
            int idx = skillMenu.selectedSkillIndex;
            skillMenu.done = false;
            playerMenu.UsingSkill = false;

            TryUseSkill(idx);
            return;
        }

        // Attack mode
        if (playerMenu.Attacking && playerMenu.index == index)
        {
            if (!attacking)
            {
                cd.currentData.stats = stats;
                attacking = true;
                attackMenu.Setup();
                gridManager.ResetAllGroundTileColors();
            }

            TryAttack();
            return;
        }

        if (!playerMenu.gameObject.activeSelf && !playerMenu.Moving && !playerMenu.Attacking && !playerMenu.UsingSkill && viewing)
        {
            gridManager.ResetAllGroundTileColors();
            viewing = false;
        }

        if (playerMenu.index != -1 && playerMenu.index != index)
            return;

        if (stateMachine.stateRunning || hasMoved) return;

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePx = Mouse.current.position.ReadValue();
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(mousePx);
        worldClick.z = 0f;

        Vector2Int clickGrid = gridManager.GetGridPos(worldClick);
        Vector2Int myGrid = gridManager.GetGridPos(transform.position);

        if (!playerMenu.gameObject.activeSelf && clickGrid == myGrid)
        {
            playerMenu.Setup(
                index,
                stats.AttackPower,
                stats.MoveDistance,
                stats.CurrentHealth,
                stats.MaxHealth,
                stats.weaponName);

            ViewMove();
            viewing = true;
            return;
        }

        StatsUpdater clickedEnemyStats = null;
        foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            Vector2Int enemyGrid = gridManager.GetGridPos(enemy.transform.position);
            if (enemyGrid == clickGrid)
            {
                clickedEnemyStats = enemy.GetComponent<StatsUpdater>();
                break;
            }
        }

        if (!playerMenu.gameObject.activeSelf && clickedEnemyStats != null && playerMenu.index == -1)
        {
            playerMenu.EnemySetup(clickedEnemyStats.CurrentHealth, clickedEnemyStats.MaxHealth);
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
        Vector2Int startGrid = gridManager.GetGridPos(transform.position);

        List<Vector2Int> path = pathfinding.FindPath(startGrid, targetGrid);
        if (path == null || path.Count == 0)
            return;

        if (path.Count > stats.MoveDistance)
        {
            return;
        }


        playerMenu.Moving = false;
        playerMenu.index = -1;
        gridManager.ResetAllGroundTileColors();
        viewing = false;
        
        stateMachine.movementPath = path;
        stateMachine.currentState = CharacterState.Moving;
        hasMoved = true;
    }

    private void TryUseSkill(int skillIdx)
    {
        skillMenu.Cancel();
        playerMenu.gameObject.SetActive(false);
        gridManager.ResetAllGroundTileColors();
        viewing = false;

        // grab the SkillInstance
        var inst = stats.Skills[skillIdx];
        if (!inst.CanUse(stats))
        {
            Debug.Log("Not enough mana or skill on cooldown!");
            // optionally re-open skillMenu here
            return;
        }

        // enqueue on your state machine
        stateMachine.currentSkill = inst;
        stateMachine.currentState = CharacterState.UsingSkill;

        // consume their action
        hasMoved = true;
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
            return; // maybe remove
        }

        if (!attackMenu.done) return;

        Vector2Int unitGrid = gridManager.GetGridPos(transform.position);
        
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

           targetCells.Add(unitGrid + targetGrid);
        }

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Vector2Int eGrid = gridManager.GetGridPos(enemy.transform.position);

            if (targetCells.Contains(eGrid))
            {
                stateMachine.attackTarget = enemy;
                stateMachine.currentState = CharacterState.Attacking;
                continue;
            }
        }

        hasMoved = true;
        playerMenu.Attacking = false;
        playerMenu.index = -1;
        attacking = false;
        attackMenu.done = false;
    }

    private void ViewMove()
    {
        gridManager.ResetAllGroundTileColors();
        Vector2Int myGrid = gridManager.GetGridPos(transform.position);

        BoundsInt bounds = gridManager.ground.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                Vector3Int tile = new Vector3Int(x, y, 0);

                if (!gridManager.ground.HasTile(tile))
                    continue;

                List<Vector2Int> path = pathfinding.FindPath(myGrid, cell);
                if (path == null || path.Count == 0)
                    continue;

                if (path.Count <= stats.MoveDistance)
                    gridManager.SetGroundTileColor(cell, new Color(0f, 0f, 1f));
            }
        }
        viewing = true;
    }
}
