using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//change to displayStats Ability
public class DisplayAbility : Ability
{
    public event EventHandler<DisplayStatsChangedEventArgs> DisplayStatsChanged;
    public event EventHandler<ButtonCreatedEventArgs> ButtonCreated;

    protected List<OverlayTile> tilesInAttackRange;

    public override void CleanUp(TileGrid tileGrid)
    {
        tilesInAttackRange.ForEach(t => t.UnMark());
    }

    //Invokes DisplayStatsChanged Event given the otherUnit
    protected virtual void OnDisplayStatsChanged(CombatStats currentUnitStats, CombatStats otherUnitStats)
    {   
        if (DisplayStatsChanged != null)
            DisplayStatsChanged.Invoke(this, new DisplayStatsChangedEventArgs(currentUnitStats, otherUnitStats));
    }

    protected virtual void OnButtonCreated(IEnumerator action, string name)
    {
        if(ButtonCreated != null)
            ButtonCreated.Invoke(this, new ButtonCreatedEventArgs(action, name));
    }


    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        base.OnAbilitySelected(tileGrid);
        tilesInAttackRange = UnitReference.GetTilesInRange(tileGrid, UnitReference.EquippedWeapon.Range).Where(t => t != UnitReference.Tile).ToList();
    }

    public override void OnAbilityDeselected(TileGrid tileGrid)
    {
        base.OnAbilityDeselected(tileGrid);
    }
}

public class DisplayStatsChangedEventArgs : EventArgs
{
    public CombatStats AttackerStats;
    public CombatStats DefenderStats;

    public DisplayStatsChangedEventArgs(CombatStats attackerStats, CombatStats defenderStats)
    {
        AttackerStats = attackerStats;
        DefenderStats = defenderStats;
    }
}


public class ButtonCreatedEventArgs : EventArgs
{
    public IEnumerator ButtonAction;
    public string ButtonName;

    public ButtonCreatedEventArgs(IEnumerator buttonAction, string buttonName)
    {
        ButtonAction = buttonAction;
        ButtonName = buttonName;
    }
}
