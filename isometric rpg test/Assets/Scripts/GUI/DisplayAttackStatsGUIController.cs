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
    public Image DefenderEffectiveness;

    public Text AttackerName;
    public Text AttackerHealth;
    public Text AttackerDamage;
    public Text AttackerHitChance;
    public Text AttackerCritChance;
    public Image AttackerEffectiveness;

    public Sprite AdvantageIcon;
    public Sprite DisadvantageIcon;

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
            if (a is DisplayAttackStatsAbility)
            {
                (a as DisplayAttackStatsAbility).AbilitySelected += OnAbilitySelected;
                (a as DisplayAttackStatsAbility).AbilityDeselected += OnAbilityDeselected;
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
        if (defenderStats.Effectiveness == -1f)
        {
            DefenderEffectiveness.sprite = DisadvantageIcon;
            DefenderEffectiveness.color = Color.white;
        }
        else if (defenderStats.Effectiveness == 1f)
        {
            DefenderEffectiveness.sprite = AdvantageIcon;
            DefenderEffectiveness.color = Color.white;
        }


        AttackerName.text = attackerStats.Name;
        AttackerHealth.text = attackerStats.Hp.ToString();
        AttackerDamage.text = attackerStats.Damage.ToString();
        AttackerCritChance.text = attackerStats.Crit.ToString();
        AttackerHitChance.text = attackerStats.Hit.ToString();
        if (attackerStats.Effectiveness == -1f)
        {
            AttackerEffectiveness.sprite = DisadvantageIcon;
            AttackerEffectiveness.color = Color.white;
        }         
        else if (attackerStats.Effectiveness == 1f)
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



