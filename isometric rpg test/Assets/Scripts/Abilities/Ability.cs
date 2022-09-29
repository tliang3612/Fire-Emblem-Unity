using System;
using System.Collections;
using UnityEngine;
using System.Linq;

public abstract class Ability : MonoBehaviour
{
    //The Unit that this ability script is attached to 
    public Unit UnitReference { get; set; }

    protected virtual void Awake()
    {
        UnitReference = GetComponent<Unit>();
    }

    public IEnumerator Execute(TileGrid tileGrid, Action<TileGrid> preAction, Action<TileGrid> postAction)
    {
        yield return StartCoroutine(Act(tileGrid, preAction, postAction));
            
    }

    public IEnumerator HumanExecute(TileGrid tileGrid)
    {
        yield return Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateAbilitySelected(tileGrid, UnitReference, UnitReference.GetComponents<Ability>().ToList()));
    }

    //Method used to define whether the ability can be used - for example it could check if the unit has any movement points left to move
    public virtual IEnumerator Act(TileGrid tileGrid) { yield return 0; }

    private IEnumerator Act(TileGrid tileGrid, Action<TileGrid> preAction, Action<TileGrid> postAction)
    {
        preAction(tileGrid);
        yield return StartCoroutine(Act(tileGrid));
        postAction(tileGrid);

        yield return 0;
    }

    public virtual void OnUnitClicked(Unit unit, TileGrid tileGrid) { }
    public virtual void OnUnitHighlighted(Unit unit, TileGrid tileGrid) { }
    public virtual void OnUnitDehighlighted(Unit unit, TileGrid tileGrid) { }
    public virtual void OnUnitDestroyed(TileGrid tileGrid) { }
    public virtual void OnTileClicked(OverlayTile tile, TileGrid tileGrid) { }
    public virtual void OnTileSelected(OverlayTile tile, TileGrid tileGrid) { }
    public virtual void OnTileDeselected(OverlayTile tile, TileGrid tileGrid) { }
    public virtual void Display(TileGrid tileGrid) { }
    public virtual void CleanUp(TileGrid tileGrid) { }

    public virtual void OnAbilitySelected(TileGrid tileGrid) { }
    public virtual void OnAbilityDeselected(TileGrid tileGrid) { }
    public virtual void OnTurnStart(TileGrid tileGrid) { }
    public virtual void OnTurnEnd(TileGrid tileGrid) { }

    public virtual bool CanPerform(TileGrid tileGrid) { return false; }
}
