// Assets/Scripts/UnitStatsData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Unit Data")]
public class UnitStatsData : ScriptableObject
{
    [SerializeField] public int maxHealth = 10;
    [SerializeField] public int attackPower = 25;
    [SerializeField] public int moveDistance = 7;
}
