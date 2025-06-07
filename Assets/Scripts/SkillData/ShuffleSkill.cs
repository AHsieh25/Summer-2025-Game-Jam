using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShuffleSkill", menuName = "Skills/Shuffle")]
public class ShuffleSkill : Skill
{
    public override bool Execute(StatsUpdater user, StatsUpdater unused = null)
    {
        if (currentCharge == 0)
        {
            var allCharacters = FindObjectsByType<StatsUpdater>(FindObjectsSortMode.None);
            var gm = user.gridManager;

            // Gather all valid floor cells that are unoccupied
            var allCells = new List<Vector2Int>();
            BoundsInt bounds = gm.ground.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (gm.ground.HasTile((Vector3Int)cell) && !gm.obstacles.HasTile((Vector3Int)cell) && !gm.IsOccupied(cell))
                    {
                        allCells.Add(cell);
                    }
                }

            // Shuffle positions
            foreach (var player in allCharacters)
            {
                if (allCells.Count == 0) break;
                int idx = Random.Range(0, allCells.Count);
                Vector2Int newGrid = allCells[idx];
                allCells.RemoveAt(idx);

                player.TeleportTo(newGrid);
            }
            if (currentCharge < 0)
            {
                user.ConsumeMana(manaCost);
                user.SetCooldown();
            }
            return true;
        }
        return false; ;
    }
}
