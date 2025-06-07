using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StunSkill", menuName = "Skills/Stun")]
public class StunSkill : Skill
{
    private static readonly List<Vector2Int> baseOffsets = new()
    {
        new Vector2Int(0, 1),
    };

    private int damage = 10;

    public override bool Execute(StatsUpdater user, StatsUpdater unused = null)
    {
        GridManager gm = user.gridManager;
        Vector2Int userGrid = gm.GetGridPos(user.transform.position);

        Direction chosenDir = Direction.Up;
        List<Vector2Int> offsets = null;

        foreach (Direction dir in new[] { Direction.Up, Direction.Right, Direction.Down, Direction.Left })
        {
            // Rotate all baseOffsets into this direction
            var rotated = SkillUtils.RotateOffsets(baseOffsets, dir);

            // Check each offset for an enemy
            foreach (var off in rotated)
            {
                var tgtCell = userGrid + off;
                var obj = gm.GetUnitAt(tgtCell);
                if (obj == null)
                    continue;

                var hit = obj.GetComponent<StatsUpdater>();
                if (hit != null && hit.IsEnemy(user))
                {
                    // Found a valid target in this direction, lock in
                    chosenDir = dir;
                    offsets = rotated;
                    break;
                }
            }

            if (offsets != null)
                break;
        }

        // If no direction found with any enemy, fail
        if (offsets == null)
            return false;

        foreach (Vector2Int cell in offsets)
        {
            Vector2Int targetGrid = userGrid + cell;
            GameObject obj = gm.GetUnitAt(targetGrid);
            if (obj == null) continue;                          // nothing to hit

            StatsUpdater hit = obj.GetComponent<StatsUpdater>();
            if (hit == null || !hit.IsEnemy(user)) continue;

            hit.TakeDamage(damage);
            hit.ApplyStatusEffect(new StunEffect(1));
            user.ConsumeMana(manaCost);
            user.SetCooldown();
            return true;
        }
        return false;
    }
}
