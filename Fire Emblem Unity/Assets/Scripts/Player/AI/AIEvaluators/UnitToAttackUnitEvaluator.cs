using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitToAttackUnitEvaluator : UnitEvaluator
{
    private float bestDamage;
    private float bestAccuracy;

    public override void PreEvaluate(Unit evaluatingUnit, TileGrid tileGrid)
    {
        bestDamage = 0f;
        bestAccuracy = 0f;

        var enemyUnits = tileGrid.GetEnemyUnits(evaluatingUnit.Player);
        var enemiesInRange = enemyUnits.Where(e => tileGrid.GetManhattenDistance(evaluatingUnit.Tile, e.Tile) <= evaluatingUnit.AttackRange);

        foreach (var enemy in enemiesInRange)
        {          
            var currentDamage = GetDryAttackDamage(evaluatingUnit, enemy);
            var isEnemyDead = enemy.HitPoints - currentDamage <= 0;

            if (isEnemyDead)
            {
                bestDamage += 50;
                continue;
            }

            if (currentDamage > bestDamage)
            {
                bestDamage = currentDamage;
            }

            var currentAccuracy = GetBattleAccuracy(evaluatingUnit, enemy);
            if(currentAccuracy > bestAccuracy)
            {
                bestAccuracy = currentAccuracy;
            }
        }
    }

    public override float Evaluate(Unit unitToEvaluate, Unit evaluatingUnit, TileGrid tileGrid)
    {
        var damage = GetDryAttackDamage(evaluatingUnit, unitToEvaluate) / bestDamage + GetBattleAccuracy(evaluatingUnit, unitToEvaluate)/bestAccuracy;
        return damage;
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