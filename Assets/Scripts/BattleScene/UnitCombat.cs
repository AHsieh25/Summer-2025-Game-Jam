using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitCombat : MonoBehaviour
{
    private UnitStats stats;
    private GridManager gridManager;
    private Tilemap groundTilemap;

    void Awake()
    {
        stats = GetComponent<UnitStats>();
    }

    public void TryAttack(GameObject target)
    {
        UnitStats targetStats = target.GetComponent<UnitStats>();

        int attackPower = stats.attackPower;

        targetStats.TakeDamage(attackPower);
    }
}
