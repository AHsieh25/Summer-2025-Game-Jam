using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TackleSkill", menuName = "Skills/Tackle")]
public class TackleSkill : Skill
{
    private static readonly List<Vector2Int> baseOffsets = new()
    {
        new Vector2Int(0, 1),
    };

    private int damage = 15;

    public override bool Execute(StatsUpdater user, StatsUpdater target = null)
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
            Vector2Int pushCell = targetGrid + cell;
            if (gm.IsOccupied(pushCell))
            {
                // Teleport or move the target one tile forward
                target.TeleportTo(pushCell);
            }
            hit.ApplyStatusEffect(new RootEffect(1));
            Debug.Log("Tackle skill success");
            user.ConsumeMana(manaCost);
            user.SetCooldown();
            return true;
        }
        Debug.Log("Tackle skill fail");
        return false;
    }
}
