using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerUnit : Unit
{
    public HealingDetails HealHandler(Unit unitToHeal)
    {      
        return unitToHeal.ReceiveHealing(AttackFactor);
    }

    public bool IsUnitHealable(Unit otherUnit)
    {
        return FindObjectOfType<TileGrid>().GetManhattenDistance(Tile, otherUnit.Tile) <= AttackRange
            && otherUnit.PlayerNumber == PlayerNumber
            && ActionPoints >= 1
            && otherUnit.HitPoints > 0;
    }
}
