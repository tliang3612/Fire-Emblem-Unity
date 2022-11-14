using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerUnit : Unit
{
    public bool IsUnitHealable(Unit otherUnit)
    {
        return FindObjectOfType<TileGrid>().GetManhattenDistance(Tile, otherUnit.Tile) <= EquippedWeapon.Range
            && otherUnit.PlayerNumber == PlayerNumber
            && ActionPoints >= 1
            && otherUnit.HitPoints > 0
            && otherUnit.HitPoints < otherUnit.TotalHitPoints;
        
    }
}
