using UnityEngine;

[RequireComponent(typeof(Transform))]
public class TransformGridHelper : MonoBehaviour
{
    [HideInInspector] public Vector2Int GridPosition;
    private GridManager gridManager;

    void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        UpdateGridPos();
    }

    public void UpdateGridPos()
    {
        int ts = gridManager.TileSize;
        GridPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / ts),
            Mathf.RoundToInt(transform.position.y / ts)
        );
    }

    void LateUpdate()
    {
        // Keep in sync if unit moves
        UpdateGridPos();
    }
}
