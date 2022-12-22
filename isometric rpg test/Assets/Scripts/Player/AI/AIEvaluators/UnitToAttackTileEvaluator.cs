using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitToAttackTileEvaluator : TileEvaluator
{
    private float bestDamage;
    private float bestAccuracy;
    private List<Unit> enemyUnits;

    private Dictionary<Unit, float> damageAndAccuracy;

    public override void PreEvaluate(Unit evaluatingUnit, TileGrid tileGrid)
    {
        damageAndAccuracy = new Dictionary<Unit, float>();
        bestDamage = 0f;
        bestAccuracy = 0f;

        enemyUnits = tileGrid.GetEnemyUnits(evaluatingUnit.Player);
        foreach (var enemy in enemyUnits)
        {
            var currentDamage = GetDryAttackDamage(evaluatingUnit, enemy);
            var currentAccuracy = GetBattleAccuracy(evaluatingUnit, enemy);

            damageAndAccuracy.Add(enemy, currentDamage + currentAccuracy);

            if (currentDamage > bestDamage)
            {
                bestDamage = currentDamage;
            }

            if (currentAccuracy > bestAccuracy)
            {
                bestAccuracy = currentAccuracy;
            }
        }
    }

    public override float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        //get all enemies that evaluatingUnit can attack from tileToEvaluate
        var attackableEnemies = enemyUnits.Where(e => evaluatingUnit.IsTileAttackableFrom(tileToEvaluate, e, false)).ToList();

        //get the best damageAndAccuracy score possible for the tileToEvaluate
        var maxScore = attackableEnemies.Select(e => damageAndAccuracy[e] / (bestDamage + bestAccuracy)).Max();
        return maxScore;
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
