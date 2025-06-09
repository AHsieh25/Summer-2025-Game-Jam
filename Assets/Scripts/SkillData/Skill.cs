using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public int manaCost;
    [System.NonSerialized] public int chargeTurns;
    [HideInInspector] public int currentCharge = 0;
    [System.NonSerialized] public int cooldownTurns;
    [HideInInspector] public int currentCooldown = 0;
    // public Sprite icon; if we want an image sprite

    public void TickCooldown()
    {
        if (currentCooldown > 0) currentCooldown--;
        if (chargeTurns > 0) chargeTurns--;
    }

    public abstract bool Execute(StatsUpdater user, StatsUpdater target = null);
}
