using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHealStatsPanel : GUIPanel
{
    [Header("DisplayHealStats")]
    public Image AllySprite; 
    public Text AllyName;
    public Text AllyHealth;
    public Text AllyNewHealth;

    public void Bind(DisplayHealStatsAbility ability)
    {
        ability.AbilitySelected += OnAbilitySelected;
        ability.AbilityDeselected += OnAbilityDeselected;
        ability.DisplayHealStatsChanged += OnStatsChanged;
    }

    protected override void OnAbilitySelected(object sender, EventArgs e)
    {
        base.OnAbilitySelected(sender, e);
    }

    protected override void OnAbilityDeselected(object sender, EventArgs e)
    {
        Panel.SetActive(false);
        SetState(GUIState.InAbilitySelection);
    }

    //Set the stats for the Unit being healed
    public void OnStatsChanged(object sender, DisplayHealStatsChangedEventArgs e)
    {
        SetStats(e.healStat);
    }


    public void SetStats(HealStats healStats)
    {
        AllySprite.sprite = healStats.AllySprite;
        AllyName.text = healStats.AllyName;
        AllyHealth.text = healStats.CurrentHealthStat.ToString();
        AllyNewHealth.text = healStats.NewHealthStat.ToString();
    }

    public void ClearStats()
    {
        AllySprite.sprite = null;
        AllyName.text = " ";
        AllyHealth.text = " ";
        AllyNewHealth.text = " ";
    }
}
