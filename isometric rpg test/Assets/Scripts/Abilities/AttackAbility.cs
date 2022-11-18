using System.Collections;
using UnityEngine;

public class AttackAbility : Ability
{
    public Unit UnitToAttack { get; set; }

    protected override void Awake()
    {
        base.Awake();
        IsDisplayable = false;
    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid))
        {
            UnitReference.SetMove(UnitReference.GetDirectionToFace(UnitToAttack.transform.position));
            UnitToAttack.Tile.MarkAsAttackableTile();
            yield return new WaitForSeconds(0.7f);

            UnitReference.ConfirmMove();
            if (tileGrid.GetManhattenDistance(UnitReference.Tile, UnitToAttack.Tile) > 1)
                yield return tileGrid.StartBattle(UnitReference, UnitToAttack, BattleEvent.RangedAction);    
            else
                yield return tileGrid.StartBattle(UnitReference, UnitToAttack, BattleEvent.MeleeAction);
        }
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid)));
    }

    public override void OnAbilityDeselected(TileGrid tileGrid)
    {
        base.OnAbilityDeselected(tileGrid);
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return UnitToAttack != null;
    }
}