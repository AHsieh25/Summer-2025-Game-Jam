using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class AttackHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int index;

    [SerializeField] private CurrentData cd;

    [SerializeField] private GridManager gridManager;

    private bool hovering = false;

    private void Awake()
    {
        if (cd == null)
            Debug.LogError("AttackHover: CurrentData (cd) not assigned.", this);

        if (gridManager == null)
            Debug.LogError("AttackHover: GridManager not assigned.", this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        ResetTintedCells();
    }

    void Update()
    {
        if (cd == null || gridManager == null) return;

        UnitStats stats = cd.currentData.stats;
        if (stats == null) return;

        Tilemap ground = gridManager.ground;
        Vector3Int unitCell3 = ground.WorldToCell(stats.transform.position);
        Vector2Int unitGrid = new Vector2Int(unitCell3.x, unitCell3.y);

        if (hovering)
        {
            TintAttackCells(stats, unitGrid, true);
        }
    }

    private void TintAttackCells(UnitStats stats, Vector2Int unitGrid, bool tint)
    {
        Tilemap ground = gridManager.ground;

        foreach (Vector2Int offset in stats.attackGrid)
        {
            Vector2Int target = Vector2Int.zero;

            switch (index)
            {
                case 0: // Up
                    target = unitGrid + offset;
                    break;
                case 1: // Right
                    target = unitGrid + new Vector2Int(offset.y, offset.x);
                    break;
                case 2: // Down
                    target = unitGrid + new Vector2Int(offset.x, -offset.y);
                    break;
                case 3: // Left
                    var rot = new Vector2Int(offset.y, offset.x);
                    target = unitGrid + new Vector2Int(-rot.x, rot.y);
                    break;
                default:
                    continue;
            }

            Vector3Int cell3 = new Vector3Int(target.x, target.y, 0);

            if (!ground.HasTile(cell3))
                continue;

            if (tint)
            {
                gridManager.SetGroundTileColor(target, Color.red);
            }
            else
            {
                gridManager.ResetGroundTileColor(target);
            }
        }
    }

    private void ResetTintedCells()
    {
        UnitStats stats = cd.currentData.stats;
        if (stats == null) return;

        Tilemap ground = gridManager.ground;
        Vector3Int unitCell3 = ground.WorldToCell(stats.transform.position);
        Vector2Int unitGrid = new Vector2Int(unitCell3.x, unitCell3.y);

        TintAttackCells(stats, unitGrid, false);
    }
}
