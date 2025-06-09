using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlastSkill", menuName = "Skills/Blast")]
public class BlastSkill : Skill
{
    private static readonly List<Vector2Int> baseOffsets = new()
    {
        new Vector2Int(-3, 3), new Vector2Int(-2, 3), new Vector2Int(-1, 3), new Vector2Int(0, 3),
        new Vector2Int(-3, 2), new Vector2Int(-2, 2), new Vector2Int(-1, 2), new Vector2Int(0, 2),
        new Vector2Int(-3, 1), new Vector2Int(-2, 1), new Vector2Int(-1, 1), new Vector2Int(0, 1),
    };

    private int damage = 75;

    public override bool Execute(StatsUpdater user, StatsUpdater unused = null)
    {
        bool didHit = false;
        if (currentCharge == 0)
        {
            GridManager gm = user.gridManager;
            Vector2Int userGrid = gm.GetGridPos(user.transform.position);

            List<Vector2Int> attackRange = SkillUtils.RotateOffsets(baseOffsets, Direction.Up);
            attackRange.AddRange(SkillUtils.RotateOffsets(baseOffsets, Direction.Right));
            attackRange.AddRange(SkillUtils.RotateOffsets(baseOffsets, Direction.Down));
            attackRange.AddRange(SkillUtils.RotateOffsets(baseOffsets, Direction.Left));

            foreach (Vector2Int cell in attackRange)
            {
                Vector2Int targetGrid = userGrid + cell;
                GameObject obj = gm.GetUnitAt(targetGrid);
                if (obj == null) continue;                          // nothing to hit

                StatsUpdater hit = obj.GetComponent<StatsUpdater>();
                if (hit == null || !hit.IsEnemy(user)) continue;

                hit.TakeDamage(damage);
                didHit = true;
            }
        }
        if (didHit)
        {
            if (currentCharge < 0)
            {
                user.ConsumeMana(manaCost);
                user.SetCooldown();
            }
        }
        return didHit;
    }
}
