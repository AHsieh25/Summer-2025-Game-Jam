using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitCombat : MonoBehaviour
{
    private StatsUpdater stats;
    private GridManager gridManager;
    private Tilemap groundTilemap;

    void Awake()
    {
        stats = GetComponent<StatsUpdater>();
    }

    public void TryAttack(GameObject target)
    {
        StatsUpdater targetStats = target.GetComponent<StatsUpdater>();

        int attackPower = stats.AttackPower;

        targetStats.TakeDamage(attackPower);
    }
}
