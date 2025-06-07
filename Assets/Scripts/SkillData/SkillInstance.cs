public class SkillInstance
{
    public Skill baseSkill;
    public int currentCooldown;
    public int currentCharge;

    public SkillInstance(Skill skill)
    {
        baseSkill = skill;
        currentCooldown = 0;
        currentCharge = 0;
    }

    public bool CanUse(StatsUpdater user)
    {
        return user.CurrentMana >= baseSkill.manaCost &&
               currentCooldown == 0 &&
               currentCharge == 0;
    }

    public void SetCooldown()
    {
        currentCooldown = baseSkill.cooldownTurns;
        currentCharge = baseSkill.chargeTurns;
    }

    public void TickCooldown()
    {
        if (currentCooldown > 0) currentCooldown--;
        if (currentCharge > 0) currentCharge--;
    }

    public bool Execute(StatsUpdater user, StatsUpdater target = null)
    {
        bool success = baseSkill.Execute(user, target);
        if (success)
        {
            SetCooldown();
        }
        return success;
    }
}
