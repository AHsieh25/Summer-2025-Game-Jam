using UnityEngine;
using System.Collections.Generic;

public class UnitStatsData : ScriptableObject
{
    public int maxHealth;
    public int attackPower;
    public int moveDistance;
    public int attackDistance;
    public List<Vector2Int> attackGrid;
}
