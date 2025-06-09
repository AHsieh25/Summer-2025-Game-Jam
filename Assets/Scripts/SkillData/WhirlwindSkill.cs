using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WhirlwindSkill", menuName = "Skills/Whirlwind")]
public class WhirlwindSkill : Skill
{
    private static readonly List<Vector2Int> baseOffsets = new()
    {
        new Vector2Int(-1, 1), new Vector2Int(0, -1), new Vector2Int(1, -1),
        new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0),
        new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
    };

    private int damage = 25;

    public override bool Execute(StatsUpdater user, StatsUpdater target = null)
    {
        bool didHit = false;

        GridManager gm = user.gridManager;
        Vector2Int userGrid = user.GetGridPosition();

        foreach (Vector2Int offset in baseOffsets)
        {
            Vector2Int targetGrid = userGrid + offset;
            GameObject obj = gm.GetUnitAt(targetGrid);
            if (obj == null) continue;                          // nothing to hit

            StatsUpdater hit = obj.GetComponent<StatsUpdater>();
            if (hit == null || !hit.IsEnemy(user)) continue;

            hit.TakeDamage(damage);
            didHit = true;
        }
        if (didHit)
        {
            user.ConsumeMana(manaCost);
            user.SetCooldown();
        }
        return didHit;
    }
}
