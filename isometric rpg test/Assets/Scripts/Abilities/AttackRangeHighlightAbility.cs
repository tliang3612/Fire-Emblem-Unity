using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class AttackRangeHighlightAbility : Ability
{
    List<Unit> enemiesInRange;
    List<OverlayTile> tilesInAttackRange;


    protected override void Awake()
    {
        base.Awake();
        Name = "AttackRangeHighlight";
        IsDisplayable = false;
    }

    public override void Display(TileGrid tileGrid)
    {
        if (UnitReference.ActionPoints > 0 && !UnitReference.InSelectionMenu)
        {
            tilesInAttackRange.ForEach(t => t.MarkAsAttackableTile());
        }              
    }

    public override void OnTileDeselected(OverlayTile tile, TileGrid tileGrid)
    {
        /*enemiesInRange?.ForEach(u => u.UnMark());

        var enemyUnits = tileGrid.GetEnemyUnits(tileGrid.CurrentPlayer);
        //searches for all enemy units that are attackable
        var inRange = enemyUnits.FindAll(u => UnitReference.IsUnitAttackable(u));

        inRange?.ForEach(u => u.MarkAsReachableEnemy());

        var availableDestinations = UnitReference.GetComponent<MoveAbility>().availableDestinations;
        if (UnitReference.ActionPoints > 0)
        {
            tilesInAttackRange?.ForEach(t => t.MarkAsAttackableTile());
        }*/
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        var availableDestinations = UnitReference.GetAvailableDestinations(tileGrid);

        if (availableDestinations.Count > 0)
        {
            tilesInAttackRange = UnitReference.GetTilesInAttackRange(availableDestinations, tileGrid);
        }        
        else
        {
            availableDestinations.Add(UnitReference.Tile);
            tilesInAttackRange = UnitReference.GetTilesInAttackRange(availableDestinations, tileGrid);
        }
            

    }

    public override void CleanUp(TileGrid tileGrid)
    {
        tilesInAttackRange?.ForEach(t => t.UnMark());
        enemiesInRange?.ForEach(e => e.UnMark());
    }

    public override void OnTurnEnd(TileGrid cellGrid)
    {
        enemiesInRange = null;
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return !UnitReference.InSelectionMenu && UnitReference.ActionPoints > 0;
    }
}
