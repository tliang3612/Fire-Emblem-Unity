using System.Collections.Generic;
using System.Linq;

public class AttackRangeHighlightAbility : Ability
{
    List<Unit> enemiesInRange;
    List<OverlayTile> tilesInAttackRange;

    public override void OnTileSelected(OverlayTile tile, TileGrid tileGrid)
    {
        var availableDestinations = UnitReference.GetComponent<MoveAbility>().availableDestinations;
        if (!availableDestinations.Contains(tile))
        {
            return;
        }
        var enemyUnits = tileGrid.GetEnemyUnits(tileGrid.CurrentPlayer);
        enemiesInRange = enemyUnits.FindAll(e => UnitReference.IsUnitAttackable(e));

        enemiesInRange.ForEach(u => u.MarkAsReachableEnemy());

    }

    public override void Display(TileGrid tileGrid)
    {
        if (UnitReference.ActionPoints > 0)
        {
            tilesInAttackRange.ForEach(t => t.MarkAsAttackableTile());
        }              
    }

    public override void OnTileDeselected(OverlayTile tile, TileGrid tileGrid)
    {
        enemiesInRange?.ForEach(u => u.UnMark());

        var enemyUnits = tileGrid.GetEnemyUnits(tileGrid.CurrentPlayer);
        //searches for all enemy units that are attackable
        var inRange = enemyUnits.FindAll(u => UnitReference.IsUnitAttackable(u));

        inRange?.ForEach(u => u.MarkAsReachableEnemy());

        var availableDestinations = UnitReference.GetComponent<MoveAbility>().availableDestinations;
        if (UnitReference.ActionPoints > 0)
        {
            tilesInAttackRange?.ForEach(t => t.MarkAsAttackableTile());
        }
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        var availableDestinations = UnitReference.GetComponent<MoveAbility>().availableDestinations;

        var enemyUnits = tileGrid.GetEnemyUnits(tileGrid.CurrentPlayer);
        enemiesInRange = enemyUnits.FindAll(e => UnitReference.IsUnitAttackable(e));

        tilesInAttackRange = UnitReference.GetTilesInAttackRange(availableDestinations, tileGrid);
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
}
