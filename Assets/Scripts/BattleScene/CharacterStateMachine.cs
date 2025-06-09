using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public enum CharacterState
{
    Idle,
    Moving,
    Attacking,
    UsingSkill,
    Stunned
}

public class CharacterStateMachine : MonoBehaviour
{
    [HideInInspector] public CharacterState currentState = CharacterState.Idle;
    [HideInInspector] public List<Vector2Int> movementPath;
    [HideInInspector] public GameObject attackTarget;
    [HideInInspector] public SkillInstance currentSkill;
    [HideInInspector] public StatsUpdater skillTarget;
    [HideInInspector] public bool skillSuccess = false;

    // prevents re‐entering a state while it’s still executing
    public bool stateRunning = false;  

    private GridManager gridManager;
    private Tilemap groundTilemap;
    private StatsUpdater stats;

    public float moveSpeed = 200f;

    public Sprite downSprite;
    public Sprite upSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

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
        gridManager.RegisterUnit(myGrid, gameObject);
    }

    void Update()
    {
        PerformState();
    }

    private void PerformState()
    {
        if (!stats.CanAct && currentState != CharacterState.Stunned)
        {
            Debug.Log($"{name} is stunned and cannot act");
            currentState = CharacterState.Stunned;
        }

        if (stateRunning) return;

        switch (currentState)
        {
            case CharacterState.Idle:
                // nothing to do
                break;
            case CharacterState.Stunned:
                Debug.Log($"{name} skipping turn because of stun.");
                currentState = CharacterState.Idle;
                break;
            case CharacterState.Moving:
                // Move character
                if (movementPath != null && movementPath.Count > 0)
                {
                    Debug.Log($"[{name}] Starting MoveAlongPath with {movementPath.Count} steps.");
                    StartCoroutine(MoveAlongPath(movementPath));
                }
                else
                {
                    Debug.Log($"[{name}] movementPath is empty or null—going back to Idle.");
                    currentState = CharacterState.Idle;
                }
                break;
            case CharacterState.Attacking:
                // Handle the attack (attackTarget must already be set)
                HandleAttackState();
                break;
            case CharacterState.UsingSkill:
                HandleSkillState();
                break;
        }
    }

    private IEnumerator MoveAlongPath(List<Vector2Int> path)
    {
        stateRunning = true;
                
        Vector2Int prevGrid = gridManager.GetGridPos(transform.position);
        gridManager.UnregisterUnit(prevGrid);

        foreach (Vector2Int gridCell in path)
        {
            // Figure out which Move state we’re in for animation purposes
            Vector3 worldTarget = groundTilemap.GetCellCenterWorld((Vector3Int)gridCell);

            Vector2Int aa = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            changeSprite(aa - new Vector2Int((int)worldTarget.x, (int)worldTarget.y));

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

        gridManager.RegisterUnit(prevGrid, gameObject);
        movementPath.Clear();
        // set animation to idle
        // Return to Idle
        currentState = CharacterState.Idle;
        stateRunning = false;
    }

    private void changeSprite(Vector2Int step)
    {
        if (step.y > 0)
        {
            GetComponent<SpriteRenderer>().sprite = downSprite;
        }
        else if (step.y < 0)
        {
            GetComponent<SpriteRenderer>().sprite = upSprite;
        }
        else if (step.x < 0)
        {
            GetComponent<SpriteRenderer>().sprite = rightSprite;
        }
        else if (step.x > 0)
        {
            GetComponent<SpriteRenderer>().sprite = leftSprite;
        }
    }

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

    private void HandleSkillState()
    {
        stateRunning = true;

        if (currentSkill != null)
        {
            skillSuccess = currentSkill.Execute(stats, skillTarget);
        }

        currentSkill = null;
        skillTarget = null;
        currentState = CharacterState.Idle;
        stateRunning = false;
    }
}
