using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonSkill", menuName = "Skills/Summon")]
public class SummonSkill : Skill
{
    public GameObject goonPrefab; // assign the enemy prefab in Inspector
    private static readonly List<Vector2Int> baseOffsets = new()
    {
        new Vector2Int(-1, 1), new Vector2Int(0, -1), new Vector2Int(1, -1),
        new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0),
        new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
    };

    public override bool Execute(StatsUpdater user, StatsUpdater unused = null)
    {
        if (currentCharge ==  0)
        {
            Vector2Int userGrid = user.gridManager.GetGridPos(user.transform.position);

            foreach (Vector2Int cell in baseOffsets)
            {
                Vector2Int targetGrid = userGrid + cell;
                // Check if valid
                if (!user.gridManager.IsOccupied(targetGrid))
                {
                    Vector3 worldPos = user.gridManager.ground.GetCellCenterWorld((Vector3Int)targetGrid);
                    GameObject.Instantiate(goonPrefab, worldPos, Quaternion.identity);
                }
            }
            if (currentCharge < 0)
            {
                user.ConsumeMana(manaCost);
                user.SetCooldown();
            }
            return true;
        }
        return false;
    }
}
