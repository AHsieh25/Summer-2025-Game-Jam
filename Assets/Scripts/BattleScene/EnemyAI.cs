using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.Rendering.DebugUI;

enum Facing { Up, Right, Down, Left }

public class EnemyAI : MonoBehaviour
{
    public GameObject parent; // List of players

    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinding pathfinding;

    private CharacterStateMachine stateMachine;
    private StatsUpdater stats;
    public HashSet<Vector2Int> reservedTiles;
    void Awake()
    {
        stateMachine = GetComponent<CharacterStateMachine>();
        stats = GetComponent<StatsUpdater>();
        reservedTiles = new HashSet<Vector2Int>();
    }

    // Enemy attacks if player is in attack grid, otherwise, moves towards closest player and attacks if player is in attack grid
    public IEnumerator TakeTurn()
    {
        if (!stats.CanAct)
        {
            yield break;
        }

        yield return StartCoroutine(TryUseRandomSkill());
        if (stateMachine.skillSuccess)
        {
            yield break;
        }

        if (TryAttack())
        {
            yield break;
        }

        // Finds closest player to move towards
        Transform closestPlayer = FindClosestPlayer();
        if (closestPlayer == null)
            yield break;

        // Set grid positions of enemy and player
        Vector2Int startGrid = gridManager.GetGridPos(transform.position);
        Vector2Int playerGrid = gridManager.GetGridPos(closestPlayer.position);

        List<Vector2Int> bestPath = FindBestPathToAdjacentCell(startGrid, playerGrid);
        if (bestPath == null || bestPath.Count == 0)
            yield break;

        int steps = Mathf.Min(stats.MoveDistance, bestPath.Count);
        List<Vector2Int> limitedPath = bestPath.GetRange(0, steps);

        stateMachine.movementPath = limitedPath;
        stateMachine.currentState = CharacterState.Moving;
        yield return null;

        while (!stateMachine.stateRunning)
            yield return null;

        while (stateMachine.stateRunning)
            yield return null;

        // Tries to attack player in range
        yield return StartCoroutine(TryUseRandomSkill());
        if (stateMachine.skillSuccess)
        {
            yield break;
        }

        if (TryAttack())
        {
            yield break;
        }
    }

    private IEnumerator TryUseRandomSkill()
    {
        Debug.Log("Trying random skill");
        if (stats.Skills == null || stats.Skills.Count == 0)
        {
            Debug.Log("no skills to try");
            yield break;
        }

        var usableSkills = stats.Skills
            .Where(s => s.CanUse(stats))
            .OrderBy(_ => Random.value)
            .ToList();

        if (usableSkills.Count == 0)
        {
            Debug.Log("no usable skills (out of mana or on cooldown)");
            yield break;
        }

        // Pick the first random one

        var skillInstance = usableSkills[0];
        Debug.Log(skillInstance.baseSkill.name);

        stateMachine.skillSuccess = false;
        stateMachine.currentSkill = skillInstance;
        stateMachine.currentState = CharacterState.UsingSkill;

        yield return null;

        // Wait for the skill to finish
        while (stateMachine.stateRunning)
            yield return null;

        if (stateMachine.skillSuccess)
            Debug.Log("Skill succeeded!");
        else
            Debug.Log("Skill failed or hit nothing.");
    }

    private List<Vector2Int> FindBestPathToAdjacentCell(Vector2Int start, Vector2Int targetCenter)
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        List<Vector2Int> bestPath = null;
        int bestLength = int.MaxValue;

        foreach (var dir in dirs)
        {
            Vector2Int target = targetCenter + dir;
            Vector3Int cell = (Vector3Int)target;
            if (!gridManager.ground.HasTile(cell) || gridManager.obstacles.HasTile(cell) || gridManager.IsOccupied(target))
                continue;

            List<Vector2Int> path = pathfinding.FindPath(start, target);
            if (path != null && path.Count < bestLength)
            {
                bestPath = path;
                bestLength = path.Count;
            }
        }

        return bestPath;
    }

    private Transform FindClosestPlayer()
    {
        Transform closest = null;
        float bestDistSqr = float.MaxValue;
        Vector3 myPos = transform.position;

        foreach (Transform player in parent.transform)
        {
            if (player == null) continue;
            float distSqr = (player.position - myPos).sqrMagnitude;
            if (distSqr < bestDistSqr)
            {
                bestDistSqr = distSqr;
                closest = player;
            }
        }
        return closest;
    }

    private bool TryAttack()
    {
        Vector2Int myGrid = gridManager.GetGridPos(transform.position);

        Transform closestPlayer = FindClosestPlayer();
        if (closestPlayer == null)
            return false;

        Vector2Int playerGrid = gridManager.GetGridPos(closestPlayer.position);
        int dx = playerGrid.x - myGrid.x;
        int dy = playerGrid.y - myGrid.y;

        Facing facing;

        if (Mathf.Abs(dx) > Mathf.Abs(dy))
        {
            facing = (dx > 0) ? Facing.Right : Facing.Left;
        }
        else
        {
            facing = (dy > 0) ? Facing.Up : Facing.Down;
        }

        List<Vector2Int> fullAttackGrid = new List<Vector2Int>(stats.attackGrid.Count);

        foreach (Vector2Int baseOffset in stats.attackGrid)
        {
            Vector2Int rot;
            switch (facing)
            {
                case Facing.Up:
                    rot = baseOffset;
                    break;
                case Facing.Right:
                    // (x,y) → (y, –x)
                    rot = new Vector2Int(baseOffset.y, -baseOffset.x);
                    break;
                case Facing.Down:
                    // (x,y) → (–x, –y)
                    rot = new Vector2Int(-baseOffset.x, -baseOffset.y);
                    break;
                case Facing.Left:
                    // (x,y) → (–y, x)
                    rot = new Vector2Int(-baseOffset.y, baseOffset.x);
                    break;
                default:
                    rot = baseOffset;
                    break;
            }
            fullAttackGrid.Add(rot);
        }

        foreach (Vector2Int offset in fullAttackGrid)
        {
            Vector2Int targetGrid = myGrid + offset;

            // Check each player if they occupy that grid cell
            foreach (Transform player in parent.transform)
            {
                if (player == null) continue;
                Vector2Int pGrid = gridManager.GetGridPos(player.position);

                if (pGrid == targetGrid)
                {
                    Debug.Log("Executing attack on: " + player.GetComponent<StatsUpdater>().name + " for " + stats.AttackPower);
                    stateMachine.attackTarget = player.gameObject;
                    stateMachine.currentState = CharacterState.Attacking;
                    // Found a player in one of our attack offsets—attack immediately
                    return true;
                }
            }
        }
        return false;
    }
}
