using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public bool UsingSkill { get; private set; }
    public int selectedSkillIndex = -1;
    public bool done = false;
    public bool back = false;

    private List<Button> _buttons = new List<Button>();

    private void Awake() => gameObject.SetActive(false);

    public void Setup(StatsUpdater stats)
    {
        foreach (var btn in _buttons) Destroy(btn.gameObject);
        _buttons.Clear();

        // Create one button per skill
        for (int i = 0; i < stats.Skills.Count; i++)
        {
            var inst = stats.Skills[i];
            var go = Instantiate(buttonPrefab, buttonContainer);
            var txt = go.GetComponentInChildren<Text>();
            txt.text = inst.baseSkill.skillName + $" (MP {inst.baseSkill.manaCost})";

            int index = i;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedSkillIndex = index;
                done = true;
                UsingSkill = false;
                gameObject.SetActive(false);
            });
            _buttons.Add(go.GetComponent<Button>());
        }

        var backGo = Instantiate(buttonPrefab, buttonContainer);
        backGo.GetComponentInChildren<Text>().text = "Back";
        backGo.GetComponent<Button>().onClick.AddListener(() =>
        {
            back = true;
            UsingSkill = false;
            done = false;
            selectedSkillIndex = -1;
            gameObject.SetActive(false);
            Cancel();
        });
        _buttons.Add(backGo.GetComponent<Button>());

        done = false;
        selectedSkillIndex = -1;
        UsingSkill = true;
        gameObject.SetActive(true);
    }

    public void Cancel()
    {
        done = false;
        UsingSkill = false;
        selectedSkillIndex = -1;
        gameObject.SetActive(false);
    }
}
