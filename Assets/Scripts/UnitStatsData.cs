// Assets/Scripts/UnitStatsData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Unit Data")]
public class UnitStatsData : ScriptableObject
{
    public int maxHealth = 10;
    public int attackPower = 3;
}
