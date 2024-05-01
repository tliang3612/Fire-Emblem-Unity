using UnityEngine;

public struct CombatStats
{
    public string UnitName { get;}
    public string WeaponName { get; }
    public Sprite WeaponSprite { get; }
    public int HitStat { get; }
    public int CritStat { get; }
    public int DamageStat { get;}
    public int RangeStat { get;}
    public int HealthStat { get;}   
    public int EffectivenessStat { get;}
    public bool CanDoubleAttack { get; }

    public CombatStats(Unit attacker, Unit defender, int encounterRange) : this()
    {
        HealthStat = attacker.HitPoints;
        RangeStat = attacker.EquippedWeapon.Range;
        EffectivenessStat = attacker.GetEffectiveness(defender.UnitType, defender.EquippedWeapon.Type);
        UnitName = attacker.UnitName;

        CanDoubleAttack = (attacker.GetAttackSpeed() - defender.GetAttackSpeed()) >= 4;

        HitStat = 0;
        CritStat = 0;
        DamageStat = 0;
        WeaponName = " ";
        WeaponSprite = null;

        if (attacker.EquippedWeapon)
        {
            WeaponName = attacker.EquippedWeapon.Name;
            WeaponSprite = attacker.EquippedWeapon.Sprite; 

            if (attacker.EquippedWeapon.Range >= encounterRange)
            {
                CritStat = attacker.GetCritChance();
                DamageStat = GetTotalDamage(attacker, defender);
                HitStat = GetBattleAccuracy(attacker, defender);
            }
        }        
    }

    //Battle Accuracy formula = Accuracy – enemy’s Avoid
    public int GetBattleAccuracy(Unit attacker, Unit defender)
    {
        return Mathf.Clamp(attacker.GetHitChance(defender.UnitType, defender.EquippedWeapon.Type) - defender.GetDodgeChance(), 0, 100);
    }

    //Damage Formula = (Attack – enemy Defence) x Critical coefficient
    public int GetTotalDamage(Unit attacker, Unit defender)
    {
        return Mathf.Clamp(attacker.GetAttack(defender.UnitType, defender.EquippedWeapon.Type) - defender.GetDefense(), 0, 100);
    }
}
