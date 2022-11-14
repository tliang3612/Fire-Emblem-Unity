using UnityEngine;

public class CombatStats
{
    public string UnitName { get; protected set; }
    public int HitStat { get; protected set; }
    public int CritStat { get; protected set; }
    public int DamageStat { get; protected set; }
    public int RangeStat { get; protected set; }
    public int HealthStat { get; protected set; }
    public int EffectivenessStat { get; protected set; }

    public CombatStats(Unit attacker, Unit defender, int range)
    {
        HealthStat = attacker.HitPoints;
        RangeStat = attacker.EquippedWeapon.Range;
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
