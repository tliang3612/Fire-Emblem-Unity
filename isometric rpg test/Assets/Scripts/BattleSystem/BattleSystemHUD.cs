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

    public void SetData(Unit unit, Unit enemyUnit, BattleEvent battleEvent)
    {
        _unit = unit;

        if (battleEvent == BattleEvent.HealAction)
        {
            hitText.text = "100";
            damageText.text = _unit.GetAttack().ToString();
            critText.text = "0";
        }
        else
        {
            hitText.text = _unit.GetBattleAccuracy(enemyUnit).ToString();
            damageText.text = _unit.GetTotalDamage(enemyUnit).ToString();
            critText.text = _unit.GetCritChance().ToString();

            hpBar.SetupHP(_unit.HitPoints, _unit.TotalHitPoints);
        }
                
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHP((float)_unit.HitPoints / _unit.TotalHitPoints, _unit.TotalHitPoints);
    }

}
