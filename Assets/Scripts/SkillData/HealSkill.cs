using UnityEngine;

[CreateAssetMenu(fileName = "HealSkill", menuName = "Skills/Heal")]
public class HealSkill : Skill
{
    private int healAmount = 25;
    private int range = 5;

    public override bool Execute(StatsUpdater user, StatsUpdater target = null)
    {
        if (target == null || target.IsEnemy(user)) return false;

        Vector2Int userGrid = user.GetGridPosition();
        Vector2Int targetGrid = target.GetGridPosition();
        int dist = Mathf.Abs(userGrid.x - targetGrid.x) + Mathf.Abs(userGrid.y - targetGrid.y);
        if (dist > range) return false;

        target.Heal(healAmount);
        user.ConsumeMana(manaCost);
        user.SetCooldown();
        return true;
    }
}