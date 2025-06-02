using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StatsUpdater : MonoBehaviour
{
    [HideInInspector] public string weaponName = "";
    [HideInInspector] public UnitStats stats;
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int AttackPower { get; private set; }
    public int WeaponType { get; private set; }
    public int MoveDistance { get; private set; }
    public List<string> Skills => stats.skills;
    public List<Vector2Int> attackGrid = new List<Vector2Int>();
    private SaveData sd;
    private string identifier;

    void Awake()
    {
        sd = gameObject.AddComponent<SaveData>();
        identifier = GetComponent<Identifier>().name;
        var ally = gameObject;
        if (ally.tag.Equals("Player"))
        {
            sd.LoadPlayerData();
            string id = ally.GetComponent<Identifier>().name;;

            var saved = sd.playerData.allies.Find(a => a.name == id);
            if (saved != null)
            {
                stats.weaponType = int.Parse(saved.weaponType);
                // apply other stats if needed
            }
        }
        WeaponType = stats.weaponType;
        MaxHealth = stats.maxHealth;
        CurrentHealth = MaxHealth;
        WeaponType = stats.weaponType;
        MoveDistance = stats.moveDistance;

        ApplyWeaponStats(WeaponType);
    }

    private void ApplyWeaponStats(int weaponType)
    {
        attackGrid.Clear();

        switch (weaponType)
        {
            case 0: // Sword
                attackGrid.Add(new Vector2Int(0, 1));
                attackGrid.Add(new Vector2Int(1, 1));
                attackGrid.Add(new Vector2Int(2, 1));
                attackGrid.Add(new Vector2Int(-1, 1));
                attackGrid.Add(new Vector2Int(-2, 1));
                AttackPower = 25;
                weaponName = "Sword";
                break;

            case 1: // Spear
                attackGrid.Add(new Vector2Int(0, 1));
                attackGrid.Add(new Vector2Int(0, 2));
                attackGrid.Add(new Vector2Int(0, 3));
                attackGrid.Add(new Vector2Int(0, 4));
                attackGrid.Add(new Vector2Int(0, 5));
                AttackPower = 20;
                weaponName = "Spear";
                break;

            case 2: // Axe
                attackGrid.Add(new Vector2Int(0, 1));
                AttackPower = 50;
                weaponName = "Axe";
                break;

            case 3: // Crossbow
                attackGrid.Add(new Vector2Int(0, 10));
                attackGrid.Add(new Vector2Int(0, 9));
                AttackPower = 30;
                weaponName = "Crossbow";
                break;

            default:
                Debug.LogWarning($"UnitStats: Unknown weaponType {weaponType}, defaulting to Sword.");
                ApplyWeaponStats(0);
                break;
        }
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }

    private void Die()
    {
        Debug.Log($"{name} has died.");

        GridManager gm = FindFirstObjectByType<GridManager>();
        if (gm != null)
        {
            Vector3Int cell = gm.ground.WorldToCell(transform.position);
            Vector2Int gridPos = new Vector2Int(cell.x, cell.y);
            gm.SetOccupied(gridPos, false);
        }

        Destroy(gameObject);
    }
}
