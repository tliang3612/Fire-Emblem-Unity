using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystemHUD : MonoBehaviour
{
    [SerializeField] Text hitText;
    [SerializeField] Text damageText;
    [SerializeField] Text critText;

    [SerializeField] HealthBar hpBar;

    Unit _unit;

    public void SetData(Unit unit)
    {
        _unit = unit;
        hitText.text = unit.GetHitChance().ToString();
        damageText.text = unit.GetAttack().ToString();
        critText.text = unit.GetCritChance().ToString();
        hpBar.SetupHP((float)_unit.HitPoints / _unit.TotalHitPoints);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHP((float)_unit.HitPoints / _unit.TotalHitPoints, _unit.TotalHitPoints);
    }

}
