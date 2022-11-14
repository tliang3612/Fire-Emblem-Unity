using UnityEngine;

public class HealStats : CombatStats
{
    public HealStats(Unit healer, Unit ally, int range) : base(healer, ally, range)
    {
        HealthStat = healer.HitPoints;
        RangeStat = healer.EquippedWeapon.Range;
        EffectivenessStat = 0;
        UnitName = healer.UnitName;
        DamageStat = healer.EquippedWeapon.Attack;
        HitStat = 100;
        CritStat = 0;
    }
}
