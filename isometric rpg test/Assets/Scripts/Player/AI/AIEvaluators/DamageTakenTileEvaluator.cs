using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageTakenTileEvaluator : TileEvaluator
{
    Dictionary<OverlayTile, float> healthAfterOccupation;

    //current health is 25
    //15 dmg taken after occupying a tile
    //10 hp left, and 
    public override void PreEvaluate(HashSet<OverlayTile> availableDestinations, Unit evaluatingUnit, TileGrid tileGrid)
    {
        healthAfterOccupation = new Dictionary<OverlayTile, float>();
        var currentHealth = evaluatingUnit.HitPoints;

        var enemyUnits = tileGrid.GetEnemyUnits(evaluatingUnit.Player);

        foreach (var enemy in enemyUnits)
        {
            var tilesInEnemyRange = enemy.GetTilesInAttackRange(tileGrid);
            var intersectingTiles = availableDestinations.Intersect(tilesInEnemyRange);

            Debug.Log(intersectingTiles.Count());
            if (intersectingTiles.Count() <= 0)
                continue;
                
            foreach (var tile in intersectingTiles)
            {               
                if(!healthAfterOccupation.ContainsKey(tile))
                {
                    healthAfterOccupation[tile] = currentHealth;
                }
                var damage = GetDryAttackDamage(enemy, evaluatingUnit, tile);
                healthAfterOccupation[tile] -= damage;               
            }
        }
    }

    public override float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid cellGrid)
    {

        if (!healthAfterOccupation.ContainsKey(tileToEvaluate)) return 1;

        Debug.Log(Mathf.Clamp(healthAfterOccupation[tileToEvaluate] / evaluatingUnit.HitPoints, 0, 1));
        return Mathf.Clamp(healthAfterOccupation[tileToEvaluate] / evaluatingUnit.HitPoints, 0, 1);
    }

    private int GetDryAttackDamage(Unit attacker, Unit defender, OverlayTile tile)
    {
        //if the attacker can double attack, the multipler is *2
        int multiplier = (attacker.GetAttackSpeed() - defender.GetAttackSpeed()) >= 4 ? 2 : 1;

        return Mathf.Clamp((attacker.GetAttack(defender.UnitType, defender.EquippedWeapon.Type) - (defender.UnitDefence - tile.DefenseBoost) * multiplier), 0, 100);
    }
}
