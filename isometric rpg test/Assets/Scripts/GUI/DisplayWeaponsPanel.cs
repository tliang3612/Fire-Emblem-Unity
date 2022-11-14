using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayWeaponsPanel : GUIPanel
{
    [Header("DisplayActions")]

    public GameObject menuActions;    
    private List<GameObject> ButtonList;
    public GameObject ActionButton;

    public void Bind(DisplayWeaponsAbility ability)
    {
        ability.AbilitySelected += OnAbilitySelected;
        ability.AbilityDeselected += OnAbilityDeselected;
        ability.ButtonCreated += OnButtonAdded;
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
        base.OnAbilitySelected(sender, e);
    }

    protected override void OnAbilityDeselected(object sender, EventArgs e)
    {
        base.OnAbilityDeselected(sender, e);
    }

}
