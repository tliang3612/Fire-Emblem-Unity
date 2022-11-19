using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAttackStatsPanel : GUIPanel
{
    [Header("DefenderStats")]
    public Text DefenderName;
    public Text DefenderHealth;
    public Text DefenderDamage;
    public Text DefenderHitChance;
    public Text DefenderCritChance;
    public Image DefenderEffectiveness;
    [SerializeField] private Text DefenderWeaponName;
    [SerializeField] private Image DefenderWeapon;
    

    [Header("Attacker Stats")]
    public Text AttackerName;
    public Text AttackerHealth;
    public Text AttackerDamage;
    public Text AttackerHitChance;
    public Text AttackerCritChance;
    public Image AttackerEffectiveness;
    [SerializeField] private Image AttackerWeapon;

    [Header("Icons")]
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
        ClearStats();
    }

    protected override void OnAbilityDeselected(object sender, EventArgs e)
    {
        Panel.SetActive(false);
        SetState(GUIState.InAbilitySelection);
    }

    //Set the stats for the Unit being healed
    public void OnStatsChanged(object sender, DisplayStatsChangedEventArgs e)
    {
        ClearStats();
        SetStats(e.AttackerStats, e.DefenderStats);
    }

    public void SetStats(CombatStats attackerStats, CombatStats defenderStats)
    {
        //Set Defender stats
        DefenderName.text = defenderStats.UnitName;
        DefenderHealth.text = defenderStats.HealthStat.ToString();
        DefenderDamage.text = defenderStats.DamageStat.ToString();
        DefenderCritChance.text = defenderStats.CritStat.ToString();
        DefenderHitChance.text = defenderStats.HitStat.ToString();

        if (defenderStats.EffectivenessStat != 0)
        {
            DefenderEffectiveness.sprite = defenderStats.EffectivenessStat == 1 ? AdvantageIcon : DisadvantageIcon;
            DefenderEffectiveness.color = Color.white;
        }

        if(attackerStats.WeaponSprite != null)
        {
            DefenderWeapon.sprite = defenderStats.WeaponSprite;
            DefenderWeapon.color = Color.white;
            DefenderWeaponName.text = defenderStats.WeaponName;
        }

        /*-------------------------------------------------------------------------------------------------------------------------- */

        AttackerName.text = attackerStats.UnitName;
        AttackerHealth.text = attackerStats.HealthStat.ToString();
        AttackerDamage.text = attackerStats.DamageStat.ToString();
        AttackerCritChance.text = attackerStats.CritStat.ToString();
        AttackerHitChance.text = attackerStats.HitStat.ToString();

        if (attackerStats.EffectivenessStat != 0)
        {
            AttackerEffectiveness.sprite = attackerStats.EffectivenessStat == 1 ? AdvantageIcon : DisadvantageIcon;
            AttackerEffectiveness.color = Color.white;
        }

        if (attackerStats.WeaponSprite != null)
        {
            AttackerWeapon.sprite = attackerStats.WeaponSprite;
            AttackerWeapon.color = Color.white;
        }
    }

    public void ClearStats()
    {
        DefenderName.text = "";
        DefenderHealth.text = "_ _";
        DefenderDamage.text = "_ _";
        DefenderCritChance.text = "_ _";
        DefenderHitChance.text = "_ _";
        DefenderEffectiveness.sprite = null;
        DefenderEffectiveness.color = Color.clear;        
        DefenderWeapon.color = Color.clear;
        DefenderWeaponName.text = " ";


        AttackerName.text = "";
        AttackerHealth.text = "_ _";
        AttackerDamage.text = "_ _";
        AttackerCritChance.text = "_ _";
        AttackerHitChance.text = "_ _";
        AttackerEffectiveness.sprite = null;
        AttackerEffectiveness.color = Color.clear;
        AttackerWeapon.color = Color.clear;
    }
}



