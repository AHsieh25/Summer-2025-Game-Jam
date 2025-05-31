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

        int attackPower = stats.attackPower;

        Vector3Int myCell3D = groundTilemap.WorldToCell(transform.position);
        Vector2Int myGrid = new Vector2Int(myCell3D.x, myCell3D.y);

        Vector3Int eCell3D = groundTilemap.WorldToCell(target.transform.position);
        Vector2Int eGrid = new Vector2Int(eCell3D.x, eCell3D.y);

        Vector2Int diff = new Vector2Int(eGrid.x - myGrid.x, eGrid.y - myGrid.y);

        bool canHit = false;

        if (stats.attackGrid != null && stats.attackGrid.Count > 0)
        {
            if (stats.attackGrid.Contains(diff))
            {
                canHit = true;
            }
        }
        else
        {
            // Enemy default
            int dist = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
            if (dist == 1)
            {
                canHit = true;
            }
        }
        Debug.Log($"{name} attacks {target.name} for {attackPower} damage");
        if (!canHit)
        {
            Debug.Log($"{name} attack failed: target not in valid attackGrid or adjacency.");
            return;
        }

        targetStats.TakeDamage(attackPower);
    }
}
