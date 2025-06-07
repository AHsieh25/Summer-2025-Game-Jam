using System.Collections.Generic;
using UnityEngine;

public class RootEffect : StatusEffect
{
    private int duration;

    public RootEffect(int turns)
    {
        duration = turns;
    }

    public override void OnApply(StatsUpdater unit)
    {
        unit.CanMove = false;
    }

    public override void OnTurnEnd(StatsUpdater unit)
    {
        duration--;
        if (duration <= 0)
        {
            unit.CanMove = true;
            unit.RemoveEffect(this);
        }
    }
}
