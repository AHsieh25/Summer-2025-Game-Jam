using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AttackHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int index;
    private StatsUpdater stats;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CurrentData cd;
    public bool hovering = false;
    public bool reset = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
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
            Debug.Log(hovering);
            stats = cd.currentData.stats;
            Tilemap ground = gridManager.ground;
            Vector3Int unitCell3 = ground.WorldToCell(stats.transform.position);
            Vector2Int unitGrid = new Vector2Int(unitCell3.x, unitCell3.y);

            Vector2Int targetGrid;
            switch (index)
            {
                case 0: //up
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = unitGrid + v;
                        gridManager.SetGroundTileColor(targetGrid, Color.red);
                    }
                    break;
                case 1: //right
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = unitGrid + new Vector2Int(v.y, -v.x);
                        gridManager.SetGroundTileColor(targetGrid, Color.red);
                    }
                    break;
                case 2: //down
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = unitGrid + new Vector2Int(-v.x, -v.y);
                        gridManager.SetGroundTileColor(targetGrid, Color.red);
                    }
                    break;
                case 3: //left
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = unitGrid + new Vector2Int(-v.y, v.x);
                        gridManager.SetGroundTileColor(targetGrid, Color.red);
                    }
                    break;
            }
            reset = true;
        }
        else if (reset)
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
                        targetGrid = unitGrid + new Vector2Int(v.y, -v.x);
                        gridManager.ResetGroundTileColor(targetGrid);
                    }
                    break;
                case 2:
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = unitGrid + new Vector2Int(-v.x, -v.y);
                        gridManager.ResetGroundTileColor(targetGrid);
                    }
                    break;
                case 3:
                    foreach (Vector2Int v in stats.attackGrid)
                    {
                        targetGrid = unitGrid + new Vector2Int(-v.y, v.x);
                        gridManager.ResetGroundTileColor(targetGrid);
                    }
                    break;
            }
            reset = false;
        }
    }
}