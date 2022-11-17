using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystemHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text hitText;
    [SerializeField] Text damageText;
    [SerializeField] Text critText;

    [SerializeField] HealthBar hpBar;
    Unit _unit;

    public void SetAttackData(Unit unit, CombatStats stats)
    {
        SetBaseData(unit);
        hitText.text = stats.HitStat.ToString();
        damageText.text = stats.DamageStat.ToString();
        critText.text = stats.CritStat.ToString();                      
    }

    public void SetHealData(Unit unit, HealStats stats)
    {

        SetBaseData(unit);
        hitText.text = "100";
        damageText.text = stats.HealAmount.ToString();
        critText.text = "0";
    }

    public void SetEmptyData(Unit unit)
    {
        SetBaseData(unit);
        hitText.text = "0";
        damageText.text = "0";
        critText.text = "0";
    }

    private void SetBaseData(Unit unit)
    {
        _unit = unit;
        hpBar.SetupHP(_unit.HitPoints, _unit.TotalHitPoints);
        nameText.text = unit.UnitName;
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHP(_unit.HitPoints, _unit.TotalHitPoints);
    }

}
