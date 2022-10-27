using System.Collections;
using UnityEngine;

public class AttackAbility : Ability
{
    public Unit UnitToAttack { get; set; }

    protected override void Awake()
    {
        base.Awake();
        Name = "Attack";
        IsDisplayable = false;
    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid) && UnitReference.IsUnitAttackable(UnitToAttack))
        {
            UnitReference.SetFinshed();
            UnitReference.ConfirmMove();
            tileGrid.StartBattle(UnitReference, UnitToAttack);         
            yield return new WaitForSeconds(0.5f);
        }
        yield return 0;
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid)));
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return UnitReference.ActionPoints > 0;
    }
}