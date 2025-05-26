using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public bool isMoving;
    private GridManager gridManager;
    private int tileSize;

    void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        tileSize = gridManager.TileSize;
    }

    public IEnumerator MoveAlongPath(List<Node> path)
    {
        isMoving = true;
        foreach (Node node in path)
        {
            Vector3 target = new Vector3(node.GridPosition.x * tileSize, node.GridPosition.y * tileSize, 0);
            while ((transform.position - target).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 160f);
                yield return null;
            }

            // Snap to grid after movement
            transform.position = target;
        }
        isMoving = false;
    }
}
