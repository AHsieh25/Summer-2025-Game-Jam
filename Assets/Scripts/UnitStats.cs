using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [Tooltip("Reference to this unit’s data asset")]
    public UnitStatsData data;

    [HideInInspector] public int currentHealth;

    void Awake()
    {
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
    }
}
