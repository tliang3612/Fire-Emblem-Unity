using System.Collections;
using UnityEngine;

public class HealAbility : Ability
{
    public Unit UnitToHeal { get; set; }

    protected override void Awake()
    {
        base.Awake();
        IsDisplayable = false;
    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid))
        {
            UnitReference.ConfirmMove();
            tileGrid.StartBattle(UnitReference, UnitToHeal, BattleEvent.HealAction);
            yield return new WaitForSeconds(0.5f);
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
        return UnitToHeal != null;
    }
}