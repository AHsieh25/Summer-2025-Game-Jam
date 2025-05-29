using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    [Tooltip("How far (in tiles) this unit can attack")]
    public int attackRange = 1;

    private UnitStats stats;

    void Awake()
    {
        stats = GetComponent<UnitStats>();
    }

    /// <summary>
    /// Try to attack the target GameObject, if it has UnitStats and is within range.
    /// </summary>
    public void TryAttack(GameObject target)
    {
        if (stats == null || target == null) return;

        UnitStats targetStats = target.GetComponent<UnitStats>();
        if (targetStats == null) return;

        attackRange = stats.data.attackDistance;

        // Check Manhattan distance on grid
        Vector2Int me = stats.GetComponent<TransformGridHelper>().GridPosition;
        Vector2Int them = targetStats.GetComponent<TransformGridHelper>().GridPosition;
        int dist = Mathf.Abs(me.x - them.x) + Mathf.Abs(me.y - them.y);

        /*
        if (dist > attackRange)
        {
            Debug.Log($"{name} attack failed: target out of range ({dist} > {attackRange})");
            return;
        }
        */

        // Perform the attack
        Debug.Log($"{name} attacks {target.name} for {stats.data.attackPower} damage");
        targetStats.TakeDamage(stats.data.attackPower);
    }
}
