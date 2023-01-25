using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealthUnitEvaluator : UnitEvaluator
{

    public override void PreEvaluate(Unit evaluatingUnit, TileGrid tileGrid)
    {
        
    }

    public override float Evaluate(Unit unitToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        //if the attacking unit outranges the defending unit
        if (evaluatingUnit.AttackRange > unitToEvaluate.EquippedWeapon.Range) 
            return 1;
        else
            return (unitToEvaluate.TotalHitPoints - (float)unitToEvaluate.HitPoints) / unitToEvaluate.TotalHitPoints;
    }
}
