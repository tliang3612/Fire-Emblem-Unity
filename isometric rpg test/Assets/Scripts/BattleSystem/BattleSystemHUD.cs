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

    [SerializeField] private Text _weaponName;
    [SerializeField] private Image _weaponImage;
    [SerializeField] private Image _effectivenessArrow;

    [SerializeField] private Sprite _upArrow;
    [SerializeField] private Sprite _downArrow;

    [SerializeField] HealthBar hpBar; 
    Unit _unit;

    public void SetAttackData(Unit unit, CombatStats stats)
    {
        SetBaseData(unit);

        if (stats.EffectivenessStat != 0)
        {
            _effectivenessArrow.color = Color.white;
            _effectivenessArrow.sprite = stats.EffectivenessStat == 1 ? _upArrow : _downArrow;
        }
        else
        {
            _effectivenessArrow.sprite = null;
            _effectivenessArrow.color = Color.clear;
        }
            

        if (unit.EquippedWeapon)
        {
            _weaponImage.sprite = unit.EquippedWeapon.Sprite;
            _weaponImage.color = Color.white;
            _weaponName.text = unit.EquippedWeapon.Name;
        }
        else
        {
            HideWeaponSprite();
        }
            

        hitText.text = stats.HitStat.ToString();
        damageText.text = stats.DamageStat.ToString();
        critText.text = stats.CritStat.ToString();                      
    }

    public void SetHealData(Unit unit, HealStats stats)
    {
        SetBaseData(unit);

        if (unit.EquippedStaff)
        {
            _weaponImage.color = Color.white;
            _weaponImage.sprite = unit.EquippedStaff.Sprite;
            _weaponName.text = unit.EquippedStaff.Name;
        }
        else
        {
            HideWeaponSprite();
        }           

        hitText.text = "100";
        damageText.text = stats.HealAmount.ToString();
        critText.text = "0";
    }
    public void SetEmptyData(Unit unit)
    {
        SetBaseData(unit);

        if (unit.EquippedWeapon)
        {
            _weaponImage.sprite = unit.EquippedWeapon.Sprite;
            _weaponImage.color = Color.white;
            _weaponName.text = unit.EquippedWeapon.Name;
        }
        else
        {
            HideWeaponSprite();
        }

        hitText.text = "0";
        damageText.text = "0";
        critText.text = "0";
    }

    private void SetBaseData(Unit unit)
    {
        _effectivenessArrow.sprite = null;
        _effectivenessArrow.color = Color.clear;

        _unit = unit;
        hpBar.SetupHP(_unit.HitPoints, _unit.TotalHitPoints);
        nameText.text = unit.UnitName;
    }

    public void HideWeaponSprite()
    {
        _weaponImage.sprite = null;
        _weaponImage.color = Color.clear;
        _weaponName.text = " ";
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHP(_unit.HitPoints);
    }

}
