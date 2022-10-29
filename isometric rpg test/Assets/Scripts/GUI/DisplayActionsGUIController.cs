using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayActionsGUIController : AbilityGUIController
{
    [Header("DisplayActions")]

    public GameObject menuActions;
    private List<GameObject> ButtonList;

    public GameObject ActionButton;

    protected override void RegisterUnit(Unit unit, List<Ability> abilityList)
    {
        base.RegisterUnit(unit, abilityList);

        foreach (Ability a in abilityList)
        {
            a.AbilitySelected += OnAbilitySelected;
            a.AbilityDeselected += OnAbilityDeselected;

            if (a is DisplayActionsAbility)
            {
                (a as DisplayActionsAbility).ButtonCreated += OnButtonAdded;
            }

        }
    }

    protected void OnButtonAdded(object sender, ButtonCreatedEventArgs e)
    {
        var actionButton = Instantiate(ActionButton, menuActions.transform);
        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = e.ButtonName;
        actionButton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(e.ButtonAction));
        ButtonList.Add(actionButton);
    }

    public override void OnAbilitySelected(object sender, EventArgs e)
    {
        ButtonList = new List<GameObject>();

        if (sender is DisplayActionsAbility)
        {
            Panel.SetActive(true);
        }
    }

    public override void OnAbilityDeselected(object sender, EventArgs e)
    {
        if (sender is DisplayActionsAbility)
        {
            Panel.SetActive(false);

            foreach(GameObject button in ButtonList)
            {
                Destroy(button);
            }
        }
    }

}
