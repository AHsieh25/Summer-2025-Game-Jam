using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Stats", menuName = "Units/Unit Stats")]
public class UnitStats : ScriptableObject
{

    public string unitName;
    public int maxHealth;
    public int moveDistance;
    public int weaponType;
    public List<string> skills;
}
