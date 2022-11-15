using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAttackStatsPanel : GUIPanel
{
    [Header("DisplayAttackStats")]
    public Text DefenderName;
    public Text DefenderHealth;
    public Text DefenderDamage;
    public Text DefenderHitChance;
    public Text DefenderCritChance;
    public Image DefenderEffectiveness;

    public Text AttackerName;
    public Text AttackerHealth;
    public Text AttackerDamage;
    public Text AttackerHitChance;
    public Text AttackerCritChance;
    public Image AttackerEffectiveness;

    public Sprite AdvantageIcon;
    public Sprite DisadvantageIcon;

    public void Bind(DisplayAttackStatsAbility ability)
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

    public void SetStats(CombatStats attackerStats, CombatStats defenderStats)
    {
        DefenderName.text = defenderStats.UnitName;
        DefenderHealth.text = defenderStats.HealthStat.ToString();
        DefenderDamage.text = defenderStats.DamageStat.ToString();
        DefenderCritChance.text = defenderStats.CritStat.ToString();
        DefenderHitChance.text = defenderStats.HitStat.ToString();
        if (defenderStats.EffectivenessStat == -1f)
        {
            DefenderEffectiveness.sprite = DisadvantageIcon;
            DefenderEffectiveness.color = Color.white;
        }
        else if (defenderStats.EffectivenessStat == 1f)
        {
            DefenderEffectiveness.sprite = AdvantageIcon;
            DefenderEffectiveness.color = Color.white;
        }


        AttackerName.text = attackerStats.UnitName;
        AttackerHealth.text = attackerStats.HealthStat.ToString();
        AttackerDamage.text = attackerStats.DamageStat.ToString();
        AttackerCritChance.text = attackerStats.CritStat.ToString();
        AttackerHitChance.text = attackerStats.HitStat.ToString();
        if (attackerStats.EffectivenessStat == -1f)
        {
            AttackerEffectiveness.sprite = DisadvantageIcon;
            AttackerEffectiveness.color = Color.white;
        }         
        else if (attackerStats.EffectivenessStat == 1f)
        {
            AttackerEffectiveness.sprite = AdvantageIcon;
            AttackerEffectiveness.color = Color.white;
        }          
    }

    public void ClearStats()
    {
        DefenderName.text = "Defender";
        DefenderHealth.text = "";
        DefenderDamage.text = "";
        DefenderCritChance.text = "";
        DefenderHitChance.text = "";
        DefenderEffectiveness.color = Color.clear;
        DefenderEffectiveness.sprite = null;


        AttackerName.text = "Attacker";
        AttackerHealth.text = "";
        AttackerDamage.text = "";
        AttackerCritChance.text = "";
        AttackerHitChance.text = "";
        AttackerEffectiveness.sprite = null;
        AttackerEffectiveness.color = Color.clear;
    }
}



