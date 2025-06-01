using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AttackHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int index;
    private UnitStats stats;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CurrentData cd;
    private bool hovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        stats = cd.currentData.stats;
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }


    void Update()
    {
        if (hovering)
        {
            Tilemap ground = gridManager.ground;
            Vector3Int unitCell3 = ground.WorldToCell(stats.transform.position);
            Vector2Int unitGrid = new Vector2Int(unitCell3.x, unitCell3.y);

            Vector2Int targetGrid;
            switch (index)
            {
                case 0:
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = unitGrid + v;
                        gridManager.SetGroundTileColor(targetGrid, Color.red);
                    }
                    break;
                case 1:
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = v;
                        int temp = targetGrid.x;
                        targetGrid.x = targetGrid.y;
                        targetGrid.y = temp;
                        targetGrid = unitGrid + targetGrid;
                        gridManager.SetGroundTileColor(targetGrid, Color.red);
                    }
                    break;
                case 2:
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = v;
                        targetGrid.y = targetGrid.y * -1;
                        targetGrid = unitGrid + targetGrid;
                        gridManager.SetGroundTileColor(targetGrid, Color.red);
                    }
                    break;
                case 3:
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = v;
                        int temp = targetGrid.x;
                        targetGrid.x = targetGrid.y;
                        targetGrid.y = temp;
                        targetGrid.x = targetGrid.x * -1;
                        targetGrid = unitGrid + targetGrid;
                        gridManager.SetGroundTileColor(targetGrid, Color.red);
                    }
                    break;
            }
        }
        else
        {
            stats = cd.currentData.stats;
            Tilemap ground = gridManager.ground;
            Vector3Int unitCell3 = ground.WorldToCell(stats.transform.position);
            Vector2Int unitGrid = new Vector2Int(unitCell3.x, unitCell3.y);

            Vector2Int targetGrid;
            switch (index)
            {
                case 0:
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = unitGrid + v;
                        gridManager.ResetGroundTileColor(targetGrid);
                    }
                    break;
                case 1:
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = v;
                        int temp = targetGrid.x;
                        targetGrid.x = targetGrid.y;
                        targetGrid.y = temp;
                        targetGrid = unitGrid + targetGrid;
                        gridManager.ResetGroundTileColor(targetGrid);
                    }
                    break;
                case 2:
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = v;
                        targetGrid.y = targetGrid.y * -1;
                        targetGrid = unitGrid + targetGrid;
                        gridManager.ResetGroundTileColor(targetGrid);
                    }
                    break;
                case 3:
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = v;
                        int temp = targetGrid.x;
                        targetGrid.x = targetGrid.y;
                        targetGrid.y = temp;
                        targetGrid.x = targetGrid.x * -1;
                        targetGrid = unitGrid + targetGrid;
                        gridManager.ResetGroundTileColor(targetGrid);
                    }
                    break;
            }
        }
    }
}