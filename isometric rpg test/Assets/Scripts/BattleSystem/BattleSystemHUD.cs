using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystemHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text defText;
    [SerializeField] HealthBar hpBar;

    Unit _unit;

    public void SetData(Unit unit)
    {
        _unit = unit;
        nameText.text = unit.UnitName;
        defText.text = unit.DefenceFactor.ToString();
        hpBar.SetupHP((float)_unit.HitPoints / _unit.TotalHitPoints);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHP((float)_unit.HitPoints / _unit.TotalHitPoints, _unit.TotalHitPoints);
    }

}
