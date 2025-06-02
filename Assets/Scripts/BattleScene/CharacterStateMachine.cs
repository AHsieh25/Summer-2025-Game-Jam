using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public enum CharacterState
{
    Idle,
    Stunned,
    Moving,
    Attacking
}

public class CharacterStateMachine : MonoBehaviour
{
    [HideInInspector] public CharacterState currentState = CharacterState.Idle;
    [HideInInspector] public List<Vector2Int> movementPath;

    [HideInInspector] public GameObject attackTarget;

    // prevents re‐entering a state while it’s still executing
    public bool stateRunning = false;  

    private GridManager gridManager;
    private Tilemap groundTilemap;
    private StatsUpdater stats;

    public float moveSpeed = 200f;

    //add animator object

    void Awake()
    {
        stats = GetComponent<StatsUpdater>();
        gridManager = FindFirstObjectByType<GridManager>();
        groundTilemap = gridManager.ground;
        //get animator component
    }

    private void Start()
    {
        Vector2Int myGrid = gridManager.GetGridPos(transform.position);
        gridManager.SetOccupied(myGrid, true);
    }

    void Update()
    {
        PerformState();
    }

    private void PerformState()
    {
        if (stateRunning) return;

        switch (currentState)
        {
            case CharacterState.Idle:
                // nothing to do
                break;

            case CharacterState.Stunned:
                // Skip the turn by immediately going back to Idle
                currentState = CharacterState.Idle;
                break;

            case CharacterState.Moving:
                // Move character
                if (movementPath != null && movementPath.Count > 0)
                {
                    StartCoroutine(MoveAlongPathCoroutine(movementPath));
                }
                else
                {
                    currentState = CharacterState.Idle;
                }
                break;

            case CharacterState.Attacking:
                // Handle the attack (attackTarget must already be set)
                HandleAttackState();
                break;
        }
    }

    private IEnumerator MoveAlongPathCoroutine(List<Vector2Int> path)
    {
        stateRunning = true;
                
        Vector2Int prevGrid = gridManager.GetGridPos(transform.position);
        gridManager.SetOccupied(prevGrid, false);

        foreach (Vector2Int gridCell in path)
        {
            // Figure out which Move state we’re in for animation purposes
            Vector3 worldTarget = groundTilemap.GetCellCenterWorld((Vector3Int)gridCell);

            while ((transform.position - worldTarget).sqrMagnitude > 0.001f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    worldTarget,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }
            transform.position = worldTarget;
            prevGrid = gridCell;
        }

        gridManager.SetOccupied(prevGrid, true);

        // set animation to idle
        // Return to Idle
        currentState = CharacterState.Idle;
        stateRunning = false;
    }

    /*
    private void AnimateStepDirection(Vector2Int step)
    {
        animator.SetBool("isMoving", true);

        if (step.y > 0)
        {
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", 1f);
        }
        else if (step.y < 0)
        {
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", -1f);
        }
        else if (step.x > 0)
        {
            animator.SetFloat("moveX", 1f);
            animator.SetFloat("moveY", 0f);
        }
        else if (step.x < 0)
        {
            animator.SetFloat("moveX", -1f);
            animator.SetFloat("moveY", 0f);
        }
    }
    */

    private void HandleAttackState()
    {
        stateRunning = true;

        if (attackTarget != null)
        {
            StatsUpdater targetStats = attackTarget.GetComponent<StatsUpdater>();
            int attackPower = stats.AttackPower;
            targetStats.TakeDamage(attackPower);
        }
        currentState = CharacterState.Idle;
        stateRunning = false;
    }
}
