using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AttackHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int index;
    private UnitStats stats;
    private TransformGridHelper gridHelper;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CurrentData cd;
    private bool hovering = false;

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
            stats = cd.currentData.stats;
            gridHelper = cd.currentData.helper;

            Vector2Int targetGrid;
            switch (index)
            {
                case 0:
                    foreach (Vector2Int v in stats.data.attackGrid)
                    {
                        targetGrid = gridHelper.GridPosition + v;
                        gridManager.SetColor2(targetGrid, true);
                    }
                    break;
                case 1:
                    foreach (Vector2Int v in stats.data.attackGrid)
                    {
                        targetGrid = v;
                        int temp = targetGrid.x;
                        targetGrid.x = targetGrid.y;
                        targetGrid.y = temp;
                        targetGrid = gridHelper.GridPosition + targetGrid;
                        gridManager.SetColor2(targetGrid, true);
                    }
                    break;
                case 2:
                    foreach (Vector2Int v in stats.data.attackGrid)
                    {
                        targetGrid = v;
                        targetGrid.y = targetGrid.y * -1;
                        targetGrid = gridHelper.GridPosition + targetGrid;
                        gridManager.SetColor2(targetGrid, true);
                    }
                    break;
                case 3:
                    foreach (Vector2Int v in stats.data.attackGrid)
                    {
                        targetGrid = v;
                        int temp = targetGrid.x;
                        targetGrid.x = targetGrid.y;
                        targetGrid.y = temp;
                        targetGrid.x = targetGrid.x * -1;
                        targetGrid = gridHelper.GridPosition + targetGrid;
                        gridManager.SetColor2(targetGrid, true);
                    }
                    break;
            }
        }
        else
        {
            stats = cd.currentData.stats;
            gridHelper = cd.currentData.helper;

            Vector2Int targetGrid;
            switch (index)
            {
                case 0:
                    foreach (Vector2Int v in stats.data.attackGrid)
                    {
                        targetGrid = gridHelper.GridPosition + v;
                        gridManager.SetColor2(targetGrid, false);
                    }
                    break;
                case 1:
                    foreach (Vector2Int v in stats.data.attackGrid)
                    {
                        targetGrid = v;
                        int temp = targetGrid.x;
                        targetGrid.x = targetGrid.y;
                        targetGrid.y = temp;
                        targetGrid = gridHelper.GridPosition + targetGrid;
                        gridManager.SetColor2(targetGrid, false);
                    }
                    break;
                case 2:
                    foreach (Vector2Int v in stats.data.attackGrid)
                    {
                        targetGrid = v;
                        targetGrid.y = targetGrid.y * -1;
                        targetGrid = gridHelper.GridPosition + targetGrid;
                        gridManager.SetColor2(targetGrid, false);
                    }
                    break;
                case 3:
                    foreach (Vector2Int v in stats.data.attackGrid)
                    {
                        targetGrid = v;
                        int temp = targetGrid.x;
                        targetGrid.x = targetGrid.y;
                        targetGrid.y = temp;
                        targetGrid.x = targetGrid.x * -1;
                        targetGrid = gridHelper.GridPosition + targetGrid;
                        gridManager.SetColor2(targetGrid, false);
                    }
                    break;
            }
        }
    }
}