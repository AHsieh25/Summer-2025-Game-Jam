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
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;

    public void Setup()
    {
        up = false;
        down = false;
        left = false;
        right = false;
        done = false;
        gameObject.SetActive(true);
    }
    public void UpButton()
    {
        up = true;
        done = true;
        gameObject.SetActive(false);
    }

    public void DownButton()
    {
        down = true;
        done = true;
        gameObject.SetActive(false);
    }
    public void LeftButton()
    {
        left = true;
        done = true;
        gameObject.SetActive(false);
    }

    public void RightButton()
    {
        right = true;
        done = true;
        gameObject.SetActive(false);
    }
}
