using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenu : MonoBehaviour
{
    public bool Moving { get; set; }
    public bool Attacking { get; set; }
    public int index = -1;

    public void Setup(int i)
    {
        index = i;
        gameObject.SetActive(true);
        Moving = false;
        Attacking = false;
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
}
