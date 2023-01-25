using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public abstract class Ability : MonoBehaviour
{
    public event EventHandler AbilitySelected;
    public event EventHandler AbilityDeselected;

    protected CameraController _cameraController;



    //The Unit that this ability script is attached to 
    public Unit UnitReference { get; set; }

    //determines if the ability can be displayed as a button
    public bool IsDisplayableAsButton { get; protected set; }
    public string Name { get; protected set; }

    protected virtual void Awake()
    {
        UnitReference = GetComponentInParent<Unit>();
        IsDisplayableAsButton = false;
        _cameraController = FindObjectOfType<CameraController>();
    }

    public IEnumerator Execute(TileGrid tileGrid, Action<TileGrid> preAction, Action<TileGrid> postAction)
    {
        yield return StartCoroutine(Act(tileGrid, preAction, postAction));           
    }

    public IEnumerator TransitionAbility(TileGrid tileGrid, Ability nextAbility)
    {
        yield return Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, nextAbility));
    }

    public IEnumerator AIExecute(TileGrid tileGrid)
    {
        yield return Execute(tileGrid, _ => { }, _ => OnAbilityDeselected(tileGrid));
    }

    //Method used to define whether the ability can be used - for example it could check if the Unit has any movement points left to move
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

    public virtual void OnRightClick(TileGrid tileGrid) { }

    public virtual void Display(TileGrid tileGrid) { }
    public virtual void CleanUp(TileGrid tileGrid) { }

    //Invoked when the ability communicates with the GUI controllers
    public virtual void OnAbilitySelected(TileGrid tileGrid) { AbilitySelected?.Invoke(this, EventArgs.Empty); }
    public virtual void OnAbilityDeselected(TileGrid tileGrid) { AbilityDeselected?.Invoke(this, EventArgs.Empty); }

    public virtual void OnTurnStart(TileGrid tileGrid) { }
    public virtual void OnTurnEnd(TileGrid tileGrid) { }

    public virtual bool CanPerform(TileGrid tileGrid) { return true; }

    
}
