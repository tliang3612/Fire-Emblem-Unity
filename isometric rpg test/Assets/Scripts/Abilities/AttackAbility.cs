using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttackAbility : Ability
{
    public Unit UnitToAttack { get; set; }
    public List<Unit> enemiesInAttackRange { get; set; }

    private List<OverlayTile> tilesInAttackRange;

    protected override void Awake()
    {
        base.Awake();
        Name = "Attack";
        IsDisplayable = true;
    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid) && UnitReference.IsUnitAttackable(UnitToAttack))
        {
            tileGrid.StartBattle(UnitReference, UnitToAttack);
            UnitReference.ConfirmMove();
            yield return new WaitForSeconds(0.5f);
        }
        yield return 0;
    }
    public override void Display(TileGrid tileGrid)
    {
        tilesInAttackRange = UnitReference.GetTilesInRange(tileGrid, UnitReference.AttackRange);
        tilesInAttackRange.ForEach(t => t.MarkAsAttackableTile());
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

    public override void CleanUp(TileGrid cellGrid)
    {
        tilesInAttackRange.ForEach(t => t.UnMark());
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        var enemyUnits = tileGrid.GetEnemyUnits(tileGrid.CurrentPlayer);
        enemiesInAttackRange = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u)).ToList();

        return enemiesInAttackRange.Count > 0 && UnitReference.ActionPoints > 0;
    }

    
}


