using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitToHealUnitEvaluator : UnitEvaluator
{
    float bestHealAmount;

    public override void PreEvaluate(Unit evaluatingUnit, TileGrid tileGrid, Dictionary<Unit, float> encounterScores)
    {
        bestHealAmount = 0f;

        var allyUnits = tileGrid.GetCurrentPlayerUnits();
        var alliesInRange = allyUnits.Where(a => tileGrid.GetManhattenDistance(evaluatingUnit.Tile, a.Tile) <= evaluatingUnit.EquippedStaff.Range);

        foreach (var ally in alliesInRange)
        {
            int healAmount = GetHealAmount(evaluatingUnit, ally);

            if( healAmount > bestHealAmount )
            {
                bestHealAmount = healAmount;
            }
        }

        Debug.Log("Best heal amount: " +  bestHealAmount);
    }
    public override float Evaluate(Unit unitToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        var healAmount = GetHealAmount(evaluatingUnit, unitToEvaluate) / bestHealAmount;
        return healAmount;
    }

    private int GetHealAmount(Unit healer, Unit ally)
    {
        if (!healer.IsUnitHealable(ally))
            return 0;


        int healAmount = Mathf.Clamp(healer.EquippedStaff.HealAmount + ally.HitPoints, 0, ally.TotalHitPoints - ally.HitPoints);
        return Mathf.Clamp(healAmount, 0, healAmount);

    }
}
