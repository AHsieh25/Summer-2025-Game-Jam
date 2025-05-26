using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class PlayerMenu : MonoBehaviour
{

    public bool Moving = false;

    public void Setup()
    {
        gameObject.SetActive(true);
    }

    public void MoveButton()
    {
        gameObject.SetActive(false);
        Moving = true;
    }

    public void AttackButton()
    {

    }

    public bool CheckVisible()
    {
        return gameObject.activeSelf;
    }

    public bool CheckMoving()
    {
        return Moving;
    }

    public void ChangeMoving()
    {
        Moving = !Moving;
    }
}






