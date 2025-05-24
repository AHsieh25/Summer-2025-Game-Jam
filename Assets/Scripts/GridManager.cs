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
                    sr.color = new Color(Random.Range(0f, 100f), 255f, Random.Range(0f, 100f));
                }

                // All tiles walkable by default
                grid[pos] = new Node(pos, true);
            }
        }
    }

    public Node GetNode(Vector2Int position)
    {
        return grid.ContainsKey(position) ? grid[position] : null;
    }
}
