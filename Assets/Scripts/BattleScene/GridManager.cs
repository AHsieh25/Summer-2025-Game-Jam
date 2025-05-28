using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int tileSize = 64;
    public int TileSize => tileSize;
    public int width, height;
    public Tile tilePrefab;
    public Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    void Start()
    {
        GenerateGrid();

        // Create come untraversable tiles
        GenerateObstacles(count: 10, size: 4);
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Vector3 worldPos = new Vector3(x * tileSize, y * tileSize, 0);

                // Instantiate tile prefab
                var tile = Instantiate(tilePrefab, worldPos, Quaternion.identity);
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
    }

    void GenerateObstacles(int count, int size)
    {
        var blocked = new HashSet<Vector2Int>();
        int placed = 0;
        int maxAttempt = 500;

        while (placed < count && maxAttempt-- > 0)
        {
            // Pick a random **grid** start
            Vector2Int start = new Vector2Int(
                Random.Range(0, width),
                Random.Range(0, height)
            );

            // Skip if buffer overlap
            if (!IsAreaClear(start, blocked))
                continue;

            // Build a connected shape of `size` tiles
            var shape = new List<Vector2Int> { start };
            var tempUsed = new HashSet<Vector2Int> { start };
            int tries = 0;

            while (shape.Count < size && tries++ < 20)
            {
                Vector2Int from = shape[Random.Range(0, shape.Count)];
                foreach (var dir in CardinalDirections())
                {
                    Vector2Int next = from + dir;
                    if (!IsInBounds(next) || tempUsed.Contains(next))
                        continue;
                    if (!IsAreaClear(next, blocked))
                        continue;

                    shape.Add(next);
                    tempUsed.Add(next);
                    break;
                }
            }

            if (shape.Count < size)
                continue; // failed to build full shape

            // Mark these grid positions as blocked and color their tiles
            foreach (var gPos in shape)
            {
                blocked.Add(gPos);
                Reserve3x3Around(gPos, blocked);

                // Update pathfinding node
                if (grid.TryGetValue(gPos, out Node node))
                    node.IsWalkable = false;

                // Find the tile GameObject by name ("Tile x,y")
                var obj = GameObject.Find($"Tile {gPos.x},{gPos.y}");
                if (obj != null)
                {
                    var sr = obj.GetComponent<SpriteRenderer>();
                    if (sr != null)
                        sr.color = Color.black;
                }
            }

            placed++;
        }
    }

    bool IsInBounds(Vector2Int p) =>
        p.x >= 0 && p.x < width && p.y >= 0 && p.y < height;

    List<Vector2Int> CardinalDirections() => new List<Vector2Int>
    {
        Vector2Int.up, Vector2Int.right,
        Vector2Int.down, Vector2Int.left
    };

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

    public Node GetNode(Vector2Int position) =>
        grid.TryGetValue(position, out var node) ? node : null;

    public void SetOccupied(Vector2Int gridPos, bool occupied)
    {
        var node = GetNode(gridPos);
        if (node != null)
            node.IsOccupied = occupied;
    }
}