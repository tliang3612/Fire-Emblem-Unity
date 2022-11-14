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

    public void SetData(Unit unit, CombatStats stats)
    {
        _unit = unit;
        hpBar.SetupHP(_unit.HitPoints, _unit.TotalHitPoints);
        nameText.text = unit.UnitName;

        hitText.text = stats.HitStat.ToString();
        damageText.text = stats.DamageStat.ToString();
        critText.text = stats.CritStat.ToString();                      
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHP(_unit.HitPoints, _unit.TotalHitPoints);
    }

}
