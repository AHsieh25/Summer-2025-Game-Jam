using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

enum Facing { Up, Right, Down, Left }

public class EnemyAI : MonoBehaviour
{
    public GameObject parent; // List of players

    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinding pathfinding;

    private CharacterStateMachine stateMachine;
    private StatsUpdater stats;

    void Awake()
    {
        stateMachine = GetComponent<CharacterStateMachine>();
        stats = GetComponent<StatsUpdater>(); 
    }

    // Enemy attacks if player is in attack grid, otherwise, moves towards closest player and attacks if player is in attack grid
    public IEnumerator TakeTurnCoroutine()
    {
        Debug.Log("Enemy start state is " + stateMachine.currentState);
        // Skip turn if stunned
        if (stateMachine.currentState == CharacterState.Stunned)
        {
            Debug.Log("Enemy state is " + stateMachine.currentState);
            yield break;
        }
            
        // Tries to attack a player in range
        if (TryAttack())
        {
            Debug.Log("Enemy state is " + stateMachine.currentState);
            yield break;
        }
            

        // Finds closest player to move towards
        Transform closestPlayer = FindClosestPlayer();
        if (closestPlayer == null)
            yield break;

        // Set grid positions of enemy and player
        Vector2Int startGrid = gridManager.GetGridPos(transform.position);
        Vector2Int playerGrid = gridManager.GetGridPos(closestPlayer.position);

        Vector2Int[] dirs = {
            Vector2Int.up, Vector2Int.right,
            Vector2Int.down, Vector2Int.left
        };

        List<Vector2Int> bestPath = null;
        int bestLength = int.MaxValue;

        foreach (var d in dirs)
        {
            Vector2Int target = playerGrid + d;

            Vector3Int tCell = new Vector3Int(target.x, target.y, 0);
            if (!gridManager.ground.HasTile(tCell))
                continue;

            if (gridManager.obstacles.HasTile(tCell))
                continue;

            if (gridManager.IsOccupied(target))
                continue;

            List<Vector2Int> path = pathfinding.FindPath(startGrid, target);
            if (path != null && path.Count > 0 && path.Count < bestLength)
            {
                bestPath = path;
                bestLength = path.Count;
            }
        }

        if (bestPath == null)
        {
            yield break;
        }

        int steps = Mathf.Min(stats.MoveDistance, bestPath.Count);
        List<Vector2Int> limitedPath = bestPath.GetRange(0, steps);
        Debug.Log($"[{name}] Computed limitedPath. Count = {limitedPath.Count}");

        stateMachine.movementPath = limitedPath;
        stateMachine.currentState = CharacterState.Moving;
        Debug.Log("Enemy state is " + stateMachine.currentState);

        while (stateMachine.stateRunning)
            yield return null;

        // Tries to attack player in range
        if (TryAttack())
        {
            Debug.Log("Enemy state is " + stateMachine.currentState);
            yield break;
        }
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
