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

    protected override void RegisterUnit(Unit unit, List<Ability> unitAbilities)
    {
        base.RegisterUnit(unit, unitAbilities);

        foreach (Ability a in unitAbilities)
        {
            if (a is DisplayActionsAbility)
            {
                (a as DisplayActionsAbility).AbilitySelected += OnAbilitySelected;
                (a as DisplayActionsAbility).AbilityDeselected += OnAbilityDeselected;
                (a as DisplayActionsAbility).ButtonCreated += OnButtonAdded;
            }              
        }
    }

    protected void OnButtonAdded(object sender, ButtonCreatedEventArgs e)
    {
        var actionButton = Instantiate(ActionButton, menuActions.transform);
        actionButton.GetComponentInChildren<Text>().text = e.ButtonName;
        actionButton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(e.ButtonAction));
        ButtonList.Add(actionButton);
    }

    protected override void OnAbilitySelected(object sender, EventArgs e)
    {
        ButtonList = new List<GameObject>();
        Panel.SetActive(true);
                 
    }

    protected override void OnAbilityDeselected(object sender, EventArgs e)
    {       
        Panel.SetActive(false);
        SetState(GUIState.Clear);

        foreach (GameObject button in ButtonList)
        {
            Destroy(button);
        }      
    }

}
