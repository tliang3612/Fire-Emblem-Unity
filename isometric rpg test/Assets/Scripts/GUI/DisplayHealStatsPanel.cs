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
        ability.DisplayStatsChanged += OnStatsChanged;
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

    //Set the stats for the unit being healed
    public void OnStatsChanged(object sender, DisplayStatsChangedEventArgs e)
    {
        SetStats(e.AttackerStats, e.DefenderStats);
    }


    public void SetStats(CombatStats healerStats, CombatStats allyStats)
    {
        AllySprite.sprite = null;
        AllyName.text = allyStats.UnitName;
        AllyHealth.text = allyStats.HealthStat.ToString();
        AllyNewHealth.text = (allyStats.HealthStat + healerStats.DamageStat).ToString();
    }

    public void ClearStats()
    {
        AllySprite.sprite = null;
        AllyName.text = " ";
        AllyHealth.text = " ";
        AllyNewHealth.text = " ";
    }
}
