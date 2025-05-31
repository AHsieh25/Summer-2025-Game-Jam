using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitStats : MonoBehaviour
{
    [Header("ScriptableObject Data")]
    [Tooltip("Reference to this unit’s data asset")]
    public UnitStatsData data;

    [Header("Weapon & Grid Info")]
    [Tooltip("If this unit uses weapons (e.g. the player), set to true")]
    [SerializeField] private bool isMunc = false;

    [HideInInspector] public int currentHealth;
    [HideInInspector] public int attackPower;
    [HideInInspector] public int moveDistance;
    [HideInInspector] public int attackDistance;
    [HideInInspector] public List<Vector2Int> attackGrid = new List<Vector2Int>();

    [HideInInspector] public string weaponName = "";

    private SaveData sd;
    private int weaponType;

    void Awake()
    {
        /*
        sd = gameObject.AddComponent<SaveData>();
        if (isMunc)
        {
            sd.LoadPlayerData();
            weaponType = int.Parse(sd.playerData.weapon);

            if (int.TryParse(sd.playerData.weapon, out weaponType))
            {
                ApplyWeaponStats(weaponType);
            }
            else
            {
                Debug.LogWarning("UnitStats: Unable to parse weapon type, defaulting to 0 (Sword).");
                ApplyWeaponStats(0);
            }
        }
        */
        ApplyWeaponStats(0);
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
                attackPower = 25;
                weaponName = "Sword";
                break;

            case 1: // Spear
                attackGrid.Add(new Vector2Int(0, 1));
                attackGrid.Add(new Vector2Int(0, 2));
                attackGrid.Add(new Vector2Int(0, 3));
                attackGrid.Add(new Vector2Int(0, 4));
                attackGrid.Add(new Vector2Int(0, 5));
                attackPower = 20;
                weaponName = "Spear";
                break;

            case 2: // Axe
                attackGrid.Add(new Vector2Int(0, 1));
                attackPower = 50;
                weaponName = "Axe";
                break;

            case 3: // Crossbow
                attackGrid.Add(new Vector2Int(0, 10));
                attackGrid.Add(new Vector2Int(0, 9));
                attackPower = 30;
                weaponName = "Crossbow";
                break;

            default:
                Debug.LogWarning($"UnitStats: Unknown weaponType {weaponType}, defaulting to Sword.");
                ApplyWeaponStats(0);
                break;
        }

        currentHealth = data.maxHealth;
        attackPower = data.attackPower;
        moveDistance = data.moveDistance;
        attackDistance = data.attackDistance;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log($"{name} took {amount} damage; HP now {currentHealth}/{data.maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, data.maxHealth);
        Debug.Log($"{name} healed by {amount}; HP now {currentHealth}/{data.maxHealth}");
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
