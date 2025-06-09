public class StunEffect : StatusEffect
{
    private int duration;

    public StunEffect(int turns)
    {
        duration = turns;
    }

    public override void OnApply(StatsUpdater unit)
    {
        unit.CanAct = false; // prevents acting
    }

    public override void OnTurnEnd(StatsUpdater unit)
    {
        duration--;
        if (duration <= 0)
        {
            unit.CanAct = true;
            unit.RemoveEffect(this);
        }
    }
}