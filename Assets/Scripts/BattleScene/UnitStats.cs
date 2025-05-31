using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [Tooltip("Reference to this unit’s data asset")]
    public UnitStatsData data;
    [SerializeField] private TransformGridHelper gridHelper;

    [HideInInspector] public int currentHealth;
    [SerializeField] public List<Vector2Int> attackGrid = new List<Vector2Int>();
    [SerializeField] public int maxHealth = 10;
    [SerializeField] public int attackPower = 25;
    [SerializeField] public int moveDistance = 7;
    [SerializeField] public int attackDistance = 1;
    [SerializeField] public bool isMunc;
    private SaveData sd;
    private int weaponType;
    public string weaponName = "";

    void Awake()
    {
        sd = gameObject.AddComponent<SaveData>();
        if (isMunc)
        {
            sd.LoadPlayerData();
            weaponType = int.Parse(sd.playerData.weapon);

            switch(weaponType)
            {
                case 0:
                    attackGrid.Add(new Vector2Int(0, 1));
                    attackGrid.Add(new Vector2Int(1, 1));
                    attackGrid.Add(new Vector2Int(2, 1));
                    attackGrid.Add(new Vector2Int(-1, 1));
                    attackGrid.Add(new Vector2Int(-2, 1));
                    attackPower = 25;
                    weaponName = "Sword";
                    break;
                case 1:
                    attackGrid.Add(new Vector2Int(0, 1));
                    attackGrid.Add(new Vector2Int(0, 2));
                    attackGrid.Add(new Vector2Int(0, 3));
                    attackGrid.Add(new Vector2Int(0, 4));
                    attackGrid.Add(new Vector2Int(0, 5));
                    attackPower = 20;
                    weaponName = "Spear";
                    break;
                case 2:
                    attackGrid.Add(new Vector2Int(0, 1));
                    attackPower = 50;
                    weaponName = "Axe";
                    break;
                case 3:
                    attackGrid.Add(new Vector2Int(0, 10));
                    attackGrid.Add(new Vector2Int(0, 9));
                    attackPower = 30;
                    weaponName = "Crossbow";
                    break;
            }
        }

        data.attackGrid = attackGrid;
        data.maxHealth = maxHealth;
        data.attackPower = attackPower;
        data.moveDistance = moveDistance;
        data.attackDistance = attackDistance;
        currentHealth = data.maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, data.maxHealth);
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Optional: play animation, disable unit, or destroy
        Destroy(gameObject); // or disable movement, remove from turn system, etc.
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        gridManager.SetOccupied(gridHelper.GridPosition, false);
    }
}
