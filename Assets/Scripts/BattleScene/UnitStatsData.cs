// Assets/Scripts/UnitStatsData.cs
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(menuName = "Stats/Unit Data")]
public class UnitStatsData : ScriptableObject
{
    public int maxHealth = 10;
    public int attackPower = 25;
    public int moveDistance = 7;
    public int attackDistance = 1;
    public List<Vector2Int> attackGrid = new List<Vector2Int>();
}
