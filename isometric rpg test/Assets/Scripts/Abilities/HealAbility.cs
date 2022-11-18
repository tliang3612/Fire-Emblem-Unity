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
            Debug.Log("yes");
            UnitReference.ConfirmMove();
            yield return tileGrid.StartBattle(UnitReference, UnitToHeal, BattleEvent.HealAction);
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
        return true;
    }
}