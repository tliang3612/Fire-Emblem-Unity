using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHealStatsGUIController : AbilityGUIController
{
    [Header("DisplayHealStats")]
    public Image AllySprite; 
    public Text AllyName;
    public Text AllyHealth;
    public Text AllyNewHealth;

    protected override void OnAbilitySelected(object sender, EventArgs e)
    {
        base.OnAbilitySelected(sender, e);
        Panel.SetActive(true);
        SetState(GUIState.InAbilitySelection);
    }

    protected override void OnAbilityDeselected(object sender, EventArgs e)
    {
        Panel.SetActive(false);
        ClearStats();
        SetState(GUIState.InAbilitySelection);
    }

    protected override void RegisterUnit(Unit unit, List<Ability> unitAbilities)
    {
        base.RegisterUnit(unit, unitAbilities);

        foreach (Ability a in unitAbilities)
        {
            if (a is DisplayHealStatsAbility)
            {
                (a as DisplayHealStatsAbility).AbilitySelected += OnAbilitySelected;
                (a as DisplayHealStatsAbility).AbilityDeselected += OnAbilityDeselected;
                (a as DisplayHealStatsAbility).DisplayStatsChanged += OnStatsChanged;
            }
        }
    }

    //Set the stats for the unit being healed
    public void OnStatsChanged(object sender, DisplayStatsChangedEventArgs e)
    {
        SetStats(e.AttackerStats, e.DefenderStats);
    }


    public void SetStats(DisplayStats healerStats, DisplayStats allyStats)
    {
        AllySprite.sprite = allyStats.Portrait;
        AllyName.text = allyStats.Name;
        AllyHealth.text = allyStats.Hp.ToString();
        AllyNewHealth.text = (allyStats.Hp + healerStats.Damage).ToString();
    }

    public void ClearStats()
    {
        AllySprite.sprite = null;
        AllyName.text = " ";
        AllyHealth.text = " ";
        AllyNewHealth.text = " ";
    }
}
