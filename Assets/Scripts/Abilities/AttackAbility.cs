using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DisplayAttackStatsAbility))]
public class AttackAbility : Ability
{
    public Unit UnitToAttack { get; set; }

    protected override void Awake()
    {
        base.Awake();
        IsDisplayableAsButton = false;
    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid))
        {
            var direction = UnitReference.GetDirectionToFace(UnitToAttack.Tile.transform.position);
            UnitReference.SetState(new UnitStateMoving(UnitReference, direction));
            UnitToAttack.Tile.MarkAsAttackableTile();
            UnitToAttack.Tile.HighlightedOnUnit();

            yield return new WaitForSeconds(1f);
            UnitReference.ConfirmMove();

            if (tileGrid.GetManhattenDistance(UnitReference.Tile, UnitToAttack.Tile) > 1)
                yield return tileGrid.StartBattle(UnitReference, UnitToAttack, BattleEvent.RangedAction);    
            else
                yield return tileGrid.StartBattle(UnitReference, UnitToAttack, BattleEvent.MeleeAction);
        }
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        StartCoroutine(TransitionAbility(tileGrid, UnitReference.GetComponentInChildren<ResetAbility>()));
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