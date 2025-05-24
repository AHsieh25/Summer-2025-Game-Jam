using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width, height;
    public Tile tilePrefab;
    public Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                // Instantiate tile prefab
                var tile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                tile.name = $"Tile {x},{y}";

                tile.Init(pos); // Initialize tile with grid pos

                // Create random color for each tile
                var sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // Assign a random color
                    sr.color = new Color(0f, Random.Range(.5f, 1f), 0f);
                }

                // All tiles traversable by default
                grid[pos] = new Node(pos, true);

            }
        }
        // Create come untraversable tiles
        GenerateObstacles(count: 10, size: 4);
    }

    void GenerateObstacles(int count, int size)
    {
        HashSet<Vector2Int> blocked = new HashSet<Vector2Int>();

        int placed = 0;
        int maxAttempts = 500;

        while (placed < count && maxAttempts-- > 0)
        {
            Vector2Int start = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

            if (!IsAreaClear(start, blocked)) continue;

            List<Vector2Int> shape = new List<Vector2Int> { start };
            HashSet<Vector2Int> tempUsed = new HashSet<Vector2Int> { start };

            int tries = 0;
            while (shape.Count < size && tries < 20)
            {
                Vector2Int from = shape[Random.Range(0, shape.Count)];
                foreach (var dir in CardinalDirections().Shuffle())
                {
                    Vector2Int next = from + dir;
                    if (!IsInBounds(next)) continue;
                    if (tempUsed.Contains(next)) continue;

                    if (IsAreaClear(next, blocked))
                    {
                        shape.Add(next);
                        tempUsed.Add(next);
                        break;
                    }
                }
                tries++;
            }

            if (shape.Count < size) continue;

            // Reserve the shape and surrounding buffer
            foreach (var tile in shape)
            {
                blocked.Add(tile);
                Reserve3x3Around(tile, blocked);

                if (grid.TryGetValue(tile, out Node node))
                {
                    node.IsWalkable = false;

                    var obj = GetTileObjectAtPosition(tile);
                    if (obj != null)
                    {
                        var sr = obj.GetComponent<SpriteRenderer>();
                        if (sr != null)
                            sr.color = Color.black;
                    }
                }
            }

            placed++;
        }
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    List<Vector2Int> CardinalDirections()
    {
        return new List<Vector2Int>
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };
    }

    bool IsAreaClear(Vector2Int center, HashSet<Vector2Int> blocked)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector2Int check = new Vector2Int(center.x + dx, center.y + dy);
                if (blocked.Contains(check))
                    return false;
            }
        }
        return true;
    }

    void Reserve3x3Around(Vector2Int center, HashSet<Vector2Int> blocked)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector2Int pos = new Vector2Int(center.x + dx, center.y + dy);
                if (IsInBounds(pos))
                    blocked.Add(pos);
            }
        }
    }


    private Tile GetTileObjectAtPosition(Vector2Int pos)
    {
        // This assumes your tiles are instantiated with names like "Tile x,y"
        string tileName = $"Tile {pos.x},{pos.y}";
        GameObject tileObj = GameObject.Find(tileName);
        if (tileObj != null)
            return tileObj.GetComponent<Tile>();
        return null;
    }

    public Node GetNode(Vector2Int position)
    {
        return grid.ContainsKey(position) ? grid[position] : null;
    }
}