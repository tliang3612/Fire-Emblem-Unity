using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitToAttackTileEvaluator : TileEvaluator
{
    private float bestDamageAndAccuracy;
    private List<Unit> attackableEnemies;

    //keeps track of the damage and accuracy score of each enemy
    private Dictionary<Unit, float> damageAndAccuracy;

    /// <summary>
    /// This method initializes the damageAndAccuracy dictionary, best damageAndAccuracy score, and attackableEnemiesList
    /// <param name="availableDestinations"></param>
    /// <param name="evaluatingUnit"></param>
    /// <param name="tileGrid"></param>
    public override void PreEvaluate(HashSet<OverlayTile> availableDestinations, Unit evaluatingUnit, TileGrid tileGrid)
    {
        damageAndAccuracy = new Dictionary<Unit, float>();
        attackableEnemies = new List<Unit>();
        bestDamageAndAccuracy = 0f;

        var attackableTiles = evaluatingUnit.GetTilesInAttackRange(tileGrid);

        foreach (var t in attackableTiles)
        {            
            var enemy = t.CurrentUnit;
            //we check if the unit exists and can be attacked
            if (enemy && evaluatingUnit.PlayerNumber != enemy.PlayerNumber)
                attackableEnemies.Add(enemy);
            else
                continue;
           
            var currentDamageAndAccuracy = GetDryAttackDamage(evaluatingUnit, enemy) + GetBattleAccuracy(evaluatingUnit, enemy);

            damageAndAccuracy.Add(enemy, currentDamageAndAccuracy);

            if (currentDamageAndAccuracy > bestDamageAndAccuracy)
            {
                bestDamageAndAccuracy = currentDamageAndAccuracy;
            }     
        }
    }

    public override float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        if (attackableEnemies.Count() <= 0)
            return 0f;

        //get the best damageAndAccuracy score possible for the tileToEvaluate

        var scores = attackableEnemies.Select(e =>
        {
            var score = 0f;
            if(evaluatingUnit.IsUnitAttackableFromTile(tileToEvaluate, e, false))
            {
                score = damageAndAccuracy[e] / bestDamageAndAccuracy;
            }
            
            return score;
        });
        return scores.Max();
    }

    private int GetDryAttackDamage(Unit attacker, Unit defender)
    {
        //if the attacker can double attack, the multipler is *2
        int multiplier = (attacker.GetAttackSpeed() - defender.GetAttackSpeed()) >= 4 ? 2 : 1;

        return Mathf.Clamp((attacker.GetAttack(defender.UnitType, defender.EquippedWeapon.Type) - defender.GetDefense() * multiplier), 0, 100);
    }

    private int GetBattleAccuracy(Unit attacker, Unit defender)
    {
        return Mathf.Clamp(attacker.GetHitChance(defender.UnitType, defender.EquippedWeapon.Type) - defender.GetDefense(), 0, 100);
    }
}
