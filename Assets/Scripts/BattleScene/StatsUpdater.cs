using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class StatsUpdater : MonoBehaviour
{
    [HideInInspector] public string weaponName = "";
    public UnitStats stats;
    [HideInInspector] public GridManager gridManager;
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaxMana { get; private set; }
    public int CurrentMana { get; private set; }
    public int AttackPower { get; private set; }
    public int WeaponType { get; private set; }
    public int MoveDistance { get; private set; }
    public bool CanAct { get; set; } = true;
    public bool CanMove { get; set; } = true;
    public List<SkillInstance> Skills { get; private set; } = new List<SkillInstance>();
    private List<Skill> baseSkills => stats.skills;
    [HideInInspector] public List<Vector2Int> attackGrid = new List<Vector2Int>();
    public List<StatusEffect> activeEffects = new List<StatusEffect>();
    
    private SaveData sd;
    private string identifier;

    void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
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
        MaxMana = stats.maxMana;
        CurrentMana = MaxMana;
        WeaponType = stats.weaponType;
        MoveDistance = stats.moveDistance;

        ApplyWeaponStats(WeaponType);

        Skills = baseSkills.Select(skill => new SkillInstance(skill)).ToList();

        if (stats.skills != null)
        {
            Debug.Log("charge: " + Skills[0].baseSkill.name);
            Debug.Log("charge: " + Skills[0].baseSkill.currentCharge);
            Debug.Log("cooldown: " + Skills[0].baseSkill.currentCooldown);
            Debug.Log("mana cost calc: " + CurrentMana.ToString() + " + " + Skills[0].baseSkill.manaCost.ToString());
        }
    }

    public Vector2Int GetGridPosition()
    {
        Vector3Int cell = gridManager.ground.WorldToCell(transform.position);
        return new Vector2Int(cell.x, cell.y);
    }

    public bool IsEnemy(StatsUpdater other)
    {
        if (other == null) return false;
        return (CompareTag("Player") && other.CompareTag("Enemy")) ||
               (CompareTag("Enemy") && other.CompareTag("Player"));
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
            case 4: // Gravekeeper
                attackGrid.Add(new Vector2Int(0, 1));
                AttackPower = 60;
                weaponName = "Fists";
                break;
            case 5: // Banshee
                attackGrid.Add(new Vector2Int(0, 10));
                attackGrid.Add(new Vector2Int(0, 9));
                AttackPower = 30;
                weaponName = "Screech";
                break;
            case 6: // Goon
                attackGrid.Add(new Vector2Int(0, 1));
                AttackPower = 25;
                weaponName = "Bite";
                break;
            case 7: // TODO: Witch
                attackGrid.Add(new Vector2Int(0, 10));
                attackGrid.Add(new Vector2Int(0, 9));
                AttackPower = 30;
                weaponName = "Necromancy";
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

    public void ConsumeMana(int amount)
    {
        CurrentMana = Mathf.Max(CurrentMana - amount, 0);
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        effect.OnApply(this);
        activeEffects.Add(effect);
    }

    public void RemoveEffect(StatusEffect effect)
    {
        activeEffects.Remove(effect);
    }

    public void OnTurnEnd()
    {
        // 1) Tick all skill cooldowns
        foreach (var skill in Skills)
            skill.TickCooldown();

        // 2) Let each status effect do its per-turn logic
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].OnTurnEnd(this);
        }
    }

    public void SetCooldown()
    {
        foreach (var skill in Skills)
            skill.SetCooldown();
    }

    public void TeleportTo(Vector2Int gridPos)
    {
        Vector3 world = gridManager.ground.GetCellCenterWorld((Vector3Int)gridPos);
        transform.position = world;
    }

    private void Die()
    {
        Debug.Log($"{name} has died.");
        Vector2Int gridPos = gridManager.GetGridPos(transform.position);
        gridManager.UnregisterUnit(gridPos);
        Destroy(gameObject);
    }
}
