using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DisplayHealStatsAbility))]
public class HealAbility : Ability
{
    public Unit UnitToHeal { get; set; }

    protected override void Awake()
    {
        base.Awake();
        IsDisplayableAsButton = false;
    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid))
        {
            UnitReference.ConfirmMove();
            var direction = UnitReference.GetDirectionToFace(UnitToHeal.Tile.transform.position);
            UnitReference.SetState(new UnitStateMoving(UnitReference, direction));

            yield return new WaitForSeconds(1f);
            UnitReference.ConfirmMove();

            yield return tileGrid.StartBattle(UnitReference, UnitToHeal, BattleEvent.HealAction);
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
        return true;
    }
}