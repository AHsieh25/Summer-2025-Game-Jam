using UnityEditor;

public abstract class StatusEffect
{
    // Called immediately when applied
    public virtual void OnApply(StatsUpdater unit) { }

    // For colldowns and charged skills
    public virtual void OnTurnEnd(StatsUpdater unit) { }
}