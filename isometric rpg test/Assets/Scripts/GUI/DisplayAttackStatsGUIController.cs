using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAttackStatsGUIController : AbilityGUIController
{
    [Header("DisplayAttackStats")]
    public Text DefenderName;
    public Text DefenderHealth;
    public Text DefenderDamage;
    public Text DefenderHitChance;
    public Text DefenderCritChance;

    public Text AttackerName;
    public Text AttackerHealth;
    public Text AttackerDamage;
    public Text AttackerHitChance;
    public Text AttackerCritChance;

    
    public override void OnAbilitySelected(object sender, EventArgs e)
    {
        if (sender is DisplayAttackStatsAbility)
        {
            Panel.SetActive(true);
        }
    }

    public override void OnAbilityDeselected(object sender, EventArgs e)
    {
        if (sender is DisplayAttackStatsAbility)
        {
            Panel.SetActive(false);
            ClearStats();
        }
    }

    protected override void RegisterUnit(Unit unit, List<Ability> abilityList)
    {
        base.RegisterUnit(unit, abilityList);


        foreach (Ability a in abilityList)
        {
            a.AbilitySelected += OnAbilitySelected;
            a.AbilityDeselected += OnAbilityDeselected;

            if (a is DisplayAttackStatsAbility)
            {
                (a as DisplayAttackStatsAbility).DisplayStatsChanged += OnStatsChanged;
            }
        }
    }

    public void OnStatsChanged(object sender, DisplayStatsChangedEventArgs e)
    {
        SetStats(e.AttackerStats, e.DefenderStats);
    }


    public void SetStats(DisplayStats attackerStats, DisplayStats defenderStats)
    {
        DefenderName.text = defenderStats.Name;
        DefenderHealth.text = defenderStats.Hp.ToString();
        DefenderDamage.text = defenderStats.Damage.ToString();
        DefenderCritChance.text = defenderStats.Crit.ToString();
        DefenderHitChance.text = defenderStats.Hit.ToString();

        AttackerName.text = attackerStats.Name;
        AttackerHealth.text = attackerStats.Hp.ToString();
        AttackerDamage.text = attackerStats.Damage.ToString();
        AttackerCritChance.text = attackerStats.Crit.ToString();
        AttackerHitChance.text = attackerStats.Hit.ToString();
    }

    public void ClearStats()
    {
        DefenderName.text = "Defender";
        DefenderHealth.text = "";
        DefenderDamage.text = "";
        DefenderCritChance.text = "";
        DefenderHitChance.text = "";

        AttackerName.text = "Attacker";
        AttackerHealth.text = "";
        AttackerDamage.text = "";
        AttackerCritChance.text = "";
        AttackerHitChance.text = "";
    }
}



