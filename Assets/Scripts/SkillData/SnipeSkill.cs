using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SnipeSkill", menuName = "Skills/Snipe")]
public class SnipeSkill : Skill
{
    private int damage = 50;

    public override bool Execute(StatsUpdater user, StatsUpdater unused = null)
    {
        // Gather all valid enemies
        List<StatsUpdater> enemies = new();
        foreach (var unit in Object.FindObjectsByType<StatsUpdater>(FindObjectsSortMode.None))
        {
            if (unit.IsEnemy(user))
                enemies.Add(unit);
        }

        if (enemies.Count == 0) return false;

        // Pick one at random
        int idx = Random.Range(0, enemies.Count);
        enemies[idx].TakeDamage(damage);
        user.ConsumeMana(manaCost);
        user.SetCooldown();
        return true;
    }
}
