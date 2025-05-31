using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    public Tilemap ground;
    public Tilemap obstacle;
    public GridManager gridManager;

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        var openSet = new PriorityQueue<Vector2Int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, int>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == target)
                return ReconstructPath(cameFrom, current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (!IsWalkable(neighbor))
                    continue;

                int tentativeG = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    int fScore = tentativeG + Heuristic(neighbor, target);
                    openSet.Enqueue(neighbor, fScore);
                }
            }
        }

        return null;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        var path = new List<Vector2Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        return new[]
        {
            pos + Vector2Int.up,
            pos + Vector2Int.down,
            pos + Vector2Int.left,
            pos + Vector2Int.right
        };
    }

    // Manhattan distance
    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private bool IsWalkable(Vector2Int pos)
    {
        Vector3Int cell = new Vector3Int(pos.x, pos.y, 0);
        if (!ground.HasTile(cell))
            return false;
        if (obstacle.HasTile(cell))
            return false;
        if (gridManager.IsOccupied(pos))
            return false;
        return true;
    }


    private class PriorityQueue<T>
    {
        private readonly List<(T item, int priority)> elements = new();

        public int Count => elements.Count;

        public void Enqueue(T item, int priority)
        {
            elements.Add((item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;
            for (int i = 1; i < elements.Count; i++)
            {
                if (elements[i].priority < elements[bestIndex].priority)
                {
                    bestIndex = i;
                }
            }
            T bestItem = elements[bestIndex].item;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
}
