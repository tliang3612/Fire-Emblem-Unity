using UnityEngine;

public struct HealStats
{
    public Sprite AllySprite { get; }
    public string AllyName { get; }
    public int HealAmount { get; }
    public int CurrentHealthStat { get; }
    public int NewHealthStat { get;}

    public HealStats(Unit healer, Unit ally) : this()
    {
        AllySprite = ally.UnitBattleSprite;
        AllyName = ally.UnitName;
        HealAmount = healer.EquippedStaff.HealAmount;
        CurrentHealthStat = ally.HitPoints;
        NewHealthStat = Mathf.Clamp(ally.HitPoints + HealAmount, 0, ally.TotalHitPoints);      
    }
}
