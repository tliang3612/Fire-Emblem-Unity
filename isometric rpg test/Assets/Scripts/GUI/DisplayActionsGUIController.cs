using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayActionsGUIController : AbilityGUIController
{
    [Header("DisplayActions")]

    public GameObject menuActions;
    private List<GameObject> ButtonList;

    public GameObject ActionButton;

    protected void OnButtonAdded(object sender, ButtonCreatedEventArgs e)
    {
        var actionButton = Instantiate(ActionButton, menuActions.transform);
        actionButton.GetComponentInChildren<Text>().text = e.ButtonName;
        actionButton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(e.ButtonAction));
        ButtonList.Add(actionButton);
    }

    protected override void OnAbilitySelected(object sender, EventArgs e)
    {
        base.OnAbilitySelected(sender, e);
        SetState(GUIState.InAbilitySelection);

        ButtonList = new List<GameObject>();

        if (sender is DisplayActionsAbility)
        {
            Panel.SetActive(true);
        }
      
    }

    protected override void OnAbilityDeselected(object sender, EventArgs e)
    {
        base.OnAbilityDeselected(sender, e);
        if (sender is DisplayActionsAbility)
        {
            Panel.SetActive(false);
            SetState(GUIState.Clear);

            foreach (GameObject button in ButtonList)
            {
                Destroy(button);
            }
        }
    }


    protected override void RegisterUnit(Unit unit, List<Ability> unitAbilities)
    {
        base.RegisterUnit(unit, unitAbilities);

        foreach (Ability a in unitAbilities)
        {
            if (a is DisplayActionsAbility)
            {
                (a as DisplayActionsAbility).ButtonCreated += OnButtonAdded;
            }

        }
    }

}
