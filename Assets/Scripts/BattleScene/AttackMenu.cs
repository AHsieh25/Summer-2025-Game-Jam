using System;
using UnityEngine;
using UnityEngine.UI;

public class AttackMenu : MonoBehaviour
{
    public bool up = false;
    public bool down = false;
    public bool left = false;
    public bool right = false;
    public bool done = false;
    public bool back = false;
    [SerializeField] private AttackHover attackHoverUp;
    [SerializeField] private AttackHover attackHoverDown;
    [SerializeField] private AttackHover attackHoverRight;
    [SerializeField] private AttackHover attackHoverLeft;


    public void Setup()
    {
        up = false;
        down = false;
        left = false;
        right = false;
        gameObject.SetActive(true);
    }
    public void UpButton()
    {
        up = true;
        done = true;
        attackHoverUp.hovering = false;
        gameObject.SetActive(false);
    }

    public void DownButton()
    {
        down = true;
        done = true;
        attackHoverDown.hovering = false;
        gameObject.SetActive(false);
    }
    public void LeftButton()
    {
        left = true;
        done = true;
        attackHoverRight.hovering = false;
        gameObject.SetActive(false);
    }

    public void RightButton()
    {
        right = true;
        done = true;
        attackHoverLeft.hovering = false;
        gameObject.SetActive(false);
    }
    public void BackButton()
    {
        back = true;
        gameObject.SetActive(false);
    }

}
