using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenu : MonoBehaviour
{
    public bool Moving { get; set; }
    public bool Attacking { get; set; }
    public bool UsingSkill { get; set; }

    public int index = -1;

    [SerializeField] private Text Attack;
    [SerializeField] private Text Move;
    [SerializeField] private Text Health;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button moveButton;
    [SerializeField] private Button endButton;
    [SerializeField] private TurnManager turnManager;

    public void Setup(int i, int attack, int move, int currentHealth, int maxHealth, string weaponName)
    {
        index = i;
        gameObject.SetActive(true);
        Moving = false;
        Attacking = false;
        attackButton.gameObject.SetActive(true);
        moveButton.gameObject.SetActive(true);
        Health.text = "Health: " + (currentHealth).ToString() + "/" + (maxHealth).ToString();
        Attack.text = weaponName + "Attack: " + (attack).ToString();
    }

    public void EnemySetup(int currentHealth, int maxHealth)
    {
        index = -2;
        gameObject.SetActive(true);
        Health.text = "Health: " + (currentHealth).ToString() + "/" + (maxHealth).ToString();
        attackButton.gameObject.SetActive(false);
        moveButton.gameObject.SetActive(false);
    }

    public void MoveButton()
    {
        Moving = true;
        Attacking = false;
        gameObject.SetActive(false);
    }

    public void AttackButton()
    {
        Attacking = true;
        Moving = false;
        gameObject.SetActive(false);
    }

    public void SkillsButton()
    {
        UsingSkill = true;
        Moving = false;
        gameObject.SetActive(false);
    }

    public void BackButton()
    {
        gameObject.SetActive(false);
        index = -1;
    }

    public void EndButton()
    {
        Moving = false;
        Attacking = false;
        turnManager.skipPlayerActions = true;
        gameObject.SetActive(false);
    }
}
