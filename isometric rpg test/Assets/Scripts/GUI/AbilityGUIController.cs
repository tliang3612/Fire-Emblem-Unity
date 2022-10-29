using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityGUIController : GUIController
{
    protected override void Awake()
    {
        base.Awake();
        Panel.SetActive(false);
    }

    protected override void RegisterUnit(Unit unit, List<Ability> abilityList)
    {
        base.RegisterUnit(unit, abilityList);

    }



    public virtual void OnAbilitySelected(object sender, EventArgs e) 
    { 
        
    }

    public virtual void OnAbilityDeselected(object sender, EventArgs e) 
    { 

    }
}