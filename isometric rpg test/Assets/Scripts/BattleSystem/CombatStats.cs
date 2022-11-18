using UnityEngine;

public struct CombatStats
{
    public string UnitName { get;}
    public int HitStat { get; }
    public int CritStat { get; }
    public int DamageStat { get;}
    public int RangeStat { get;}
    public int HealthStat { get;}
    public int EffectivenessStat { get;}

    public CombatStats(Unit attacker, Unit defender, int range) : this()
    {
        HealthStat = attacker.HitPoints;
        RangeStat = attacker.EquippedWeapon.Range;
        Debug.Log(defender.EquippedWeapon);
        EffectivenessStat = attacker.GetEffectiveness(defender.EquippedWeapon.Type);
        UnitName = attacker.UnitName;

        if (attacker.EquippedWeapon.Range >= range)
        {
            CritStat = attacker.GetCritChance();
            DamageStat = GetTotalDamage(attacker, defender);
            HitStat = GetBattleAccuracy(attacker, defender);
        }
        else
        {
            HitStat = 0;
            CritStat = 0;
            DamageStat = 0;

        }
    }

    //Battle Accuracy formula = Accuracy – enemy’s Avoid
    public int GetBattleAccuracy(Unit attacker, Unit defender)
    {
        return Mathf.Clamp((attacker.GetHitChance(defender.EquippedWeapon.Type)) - defender.GetDodgeChance(), 0, 100);
    }

    //Damage Formula = (Attack – enemy Defence) x Critical coefficient
    public int GetTotalDamage(Unit attacker, Unit defender)
    {
        return (attacker.GetAttack(defender.EquippedWeapon.Type) - defender.GetDefense());
    }
}
