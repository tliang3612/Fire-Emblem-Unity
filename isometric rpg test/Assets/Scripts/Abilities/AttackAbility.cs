using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttackAbility : Ability
{
    public Unit UnitToAttack { get; set; }
    public List<Unit> enemiesInAttackRange { get; set; }
        

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid) && UnitReference.IsUnitAttackable(UnitToAttack))
        {
            tileGrid.StartBattle(UnitReference, UnitToAttack);
            yield return new WaitForSeconds(0.5f);
        }
        yield return 0;
    }
    public override void Display(TileGrid tileGrid)
    { 
        var enemyUnits = tileGrid.GetEnemyUnits(tileGrid.CurrentPlayer);
        enemiesInAttackRange = enemyUnits.Where(e => UnitReference.IsUnitAttackable(e)).ToList();
        enemiesInAttackRange.ForEach(e => e.MarkAsReachableEnemy());
    }

    public override void OnUnitClicked(Unit unit, TileGrid tileGrid)
    {
        if (UnitReference.IsUnitAttackable(unit))
        {
            UnitToAttack = unit;
            StartCoroutine(HumanExecute(tileGrid));
        }
        else if (tileGrid.GetCurrentPlayerUnits().Contains(unit))
        {
            tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, unit, unit.GetComponents<Ability>().ToList());
        }
    }

    public override void OnTileClicked(OverlayTile tile, TileGrid tileGrid)
    {
        tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid);
    }

    public override void CleanUp(TileGrid cellGrid)
    {
        enemiesInAttackRange.ForEach(u =>
        {
            if (u != null)
            {
                u.UnMark();
            }
        });
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        if (UnitReference.ActionPoints <= 0)
        {
            return false;
        }

        var enemyUnits = tileGrid.GetEnemyUnits(tileGrid.CurrentPlayer);
        enemiesInAttackRange = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u)).ToList();

        return enemiesInAttackRange.Count > 0 && UnitReference.InSelectionMenu == false;
    }

    
}


