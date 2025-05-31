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
        gridManager = FindFirstObjectByType<GridManager>();

        if (gridManager == null)
            Debug.LogError("UnitCombat: No GridManager found in scene!", this);
        else
            groundTilemap = gridManager.ground;
    }

    public void TryAttack(GameObject target)
    {
        if (stats == null || target == null || groundTilemap == null)
            return;

        UnitStats targetStats = target.GetComponent<UnitStats>();
        if (targetStats == null)
            return;

        int attackRange = stats.attackDistance;
        int attackPower = stats.attackPower;

        Vector3Int myCell3D = groundTilemap.WorldToCell(transform.position);
        Vector2Int myGrid = new Vector2Int(myCell3D.x, myCell3D.y);

        Vector3Int theirCell3D = groundTilemap.WorldToCell(target.transform.position);
        Vector2Int theirGrid = new Vector2Int(theirCell3D.x, theirCell3D.y);

        int dist = Mathf.Abs(myGrid.x - theirGrid.x) + Mathf.Abs(myGrid.y - theirGrid.y);

        Debug.Log($"{name} attempts attack on {target.name}: dist={dist}, range={attackRange}, power={attackPower}");

        if (dist > attackRange)
        {
            Debug.Log($"{name} attack failed: target out of range ({dist} > {attackRange})");
            return;
        }

        Debug.Log($"{name} attacks {target.name} for {attackPower} damage");
        targetStats.TakeDamage(attackPower);
    }
}
