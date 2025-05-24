using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public bool isMoving;

    public IEnumerator MoveAlongPath(List<Node> path)
    {
        isMoving = true;
        foreach (Node node in path)
        {
            Vector3 target = new Vector3(node.GridPosition.x, node.GridPosition.y, 0);
            while ((transform.position - target).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 5f);
                yield return null;
            }

            // Snap to grid after movement
            transform.position = target;
        }
        isMoving = false;
    }
}
