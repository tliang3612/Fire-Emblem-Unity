using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageTakenTileEvaluator : TileEvaluator
{
    Dictionary<OverlayTile, float> _healthAfterOccupation;

    //current health is 25
    //15 dmg taken after occupying a tile
    //10 hp left, and 
    public override void PreEvaluate(HashSet<OverlayTile> availableDestinations, Unit evaluatingUnit, TileGrid tileGrid, Dictionary<Unit, List<Tuple<OverlayTile, float>>> encounterScores)
    {
        _healthAfterOccupation = new Dictionary<OverlayTile, float>();
        var currentHealth = evaluatingUnit.HitPoints;

        var enemyUnits = tileGrid.GetEnemyUnits(evaluatingUnit.Player);

        foreach (var enemy in enemyUnits)
        {
            // Get all the tiles within enemies attack range
            var tilesInEnemyRange = enemy.GetTilesInAttackRange(tileGrid);
            var intersectingTiles = availableDestinations.Intersect(tilesInEnemyRange);

            if (intersectingTiles.Count() <= 0)
                continue;
                
            foreach (var tile in intersectingTiles)
            {               
                if(!_healthAfterOccupation.ContainsKey(tile))
                {
                    _healthAfterOccupation[tile] = currentHealth;
                }
                var accuracyMod = GetBattleAccuracy(enemy, evaluatingUnit, tile)/100f;
                var bestEnemyDamage = 0f;
                //We assume that the enemy will swap to the most effective weapon, so we account for each weapon the enemy has
                foreach(var weapon in enemy.AvailableWeapons)
                {
                    var originalWeapon = enemy.EquippedWeapon;
                    enemy.EquipItem(weapon);
                    var damage = GetDryAttackDamage(enemy, evaluatingUnit, tile) * accuracyMod;
                    if(damage > bestEnemyDamage)
                        bestEnemyDamage = damage;
                    enemy.EquipItem(originalWeapon);

                }

                _healthAfterOccupation[tile] -= bestEnemyDamage;               
            }
        }
    }

    public override float Evaluate(OverlayTile tileToEvaluate, Unit evaluatingUnit, TileGrid cellGrid)
    {
        if (!_healthAfterOccupation.ContainsKey(tileToEvaluate)) return .5f; // if no health is lost after occupying tile, no penalty

        return Mathf.Clamp(_healthAfterOccupation[tileToEvaluate]/evaluatingUnit.HitPoints, 0, .5f);
    }

    private int GetDryAttackDamage(Unit attacker, Unit defender, OverlayTile tile)
    {
        var originalTile = attacker.Tile;
        attacker.Tile = tile;
        //if the attacker can double attack, the multipler is *2
        int multiplier = (attacker.GetAttackSpeed() - defender.GetAttackSpeed()) >= 4 ? 2 : 1;

        var attackerAttackStat = attacker.GetAttack(defender.UnitType, defender.EquippedWeapon.Type);
        var defenderDefenseStat = defender.GetDefense();
        attacker.Tile = originalTile;
        return Mathf.Clamp((attackerAttackStat - defenderDefenseStat) * multiplier, 0, 100);
    }

    private int GetBattleAccuracy(Unit attacker, Unit defender, OverlayTile tile)
    {
        var originalTile = attacker.Tile;
        attacker.Tile = tile;
        var attackerHitStat = attacker.GetHitChance(defender.UnitType, defender.EquippedWeapon.Type);
        var defenderDodgeStat = defender.GetDodgeChance();
        attacker.Tile = originalTile;
        return Mathf.Clamp(attackerHitStat - defenderDodgeStat, 0, 100);
    }
}
