using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Enemy unit isnt doing confirm mocve poropely 
public class MoveAbility : Ability
{
    public OverlayTile Destination { get; set; }
    public List<OverlayTile> availableDestinations { get; set; }  
    public List<OverlayTile> path { get; set; }

    private List<OverlayTile> tilesInAttackRange;

    protected override void Awake()
    {
        base.Awake();
        Name = "Move";
        IsDisplayable = false;
    }


    public override IEnumerator Act(TileGrid tileGrid)
    {
        if(CanPerform(tileGrid) && availableDestinations.Contains(Destination))
        {
            var path = UnitReference.FindPath(Destination, tileGrid);
            UnitReference.Move(path);
            Debug.Log("from act:" + path.Count);
            while (UnitReference.IsMoving)
            {
                yield return 0;
            }
        }
        
        yield return 0;
    }

    public override void Display(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid))
        {
            tilesInAttackRange.ForEach(t => t.MarkAsAttackableTile());

            availableDestinations.ForEach(t => t.MarkAsReachable());
        }
    }

    public override void OnUnitClicked(Unit unit, TileGrid tileGrid)
    {
        //if unit the unit clicked was the UnitReference
        if(UnitReference == unit)
        {
            Destination = UnitReference.Tile;
            StartCoroutine(Execute(tileGrid,
                _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
                _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponent<DisplayActionsAbility>())));
        }
        else if (tileGrid.GetCurrentPlayerUnits().Contains(unit))
        {
            tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, unit, unit.GetComponent<MoveAbility>());
        }
    }

    public override void OnTileClicked(OverlayTile tile, TileGrid tileGrid)
    {
        if (availableDestinations.Contains(tile) && UnitReference.IsTileMovableTo(tile))
        {
            Destination = tile;
            StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponent<DisplayActionsAbility>())));
        }
        else
        {
            tileGrid.GridState = new TileGridStateWaitingForInput(tileGrid);
        }
    }

    public override void OnTileSelected(OverlayTile tile, TileGrid tileGrid)
    {
        if (CanPerform(tileGrid) && availableDestinations.Contains(tile) && UnitReference.IsTileMovableTo(tile))
        {
            path = UnitReference.FindPath(tile, tileGrid);
            TranslateArrows(tileGrid);       
        }
    }

    public override void OnTileDeselected(OverlayTile tile, TileGrid tileGrid)
    {
        if (CanPerform(tileGrid) && availableDestinations.Contains(tile))
        {
            availableDestinations.ForEach(t => t.MarkAsReachable());
        }
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        if(tileGrid.CurrentPlayer is HumanPlayer)
            UnitReference.SetMove();

        availableDestinations = UnitReference.GetAvailableDestinations(tileGrid);
        tilesInAttackRange = UnitReference.GetTilesInAttackRange(availableDestinations, tileGrid);
    }

    public override void OnAbilityDeselected(TileGrid tileGrid)
    {
        if (tileGrid.CurrentPlayer is HumanPlayer)
            UnitReference.SetAnimationToIdle();
    }

    public override void CleanUp(TileGrid tileGrid)
    {
        availableDestinations.ForEach(t => t.UnMark());
        tilesInAttackRange.ForEach(t => t.UnMark());
        ResetArrows();
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return UnitReference.ActionPoints > 0 && UnitReference.GetAvailableDestinations(tileGrid).Count > 1 && UnitReference.cachedPath.Count <= 0;
    }

    private void TranslateArrows(TileGrid tileGrid)
    {
        foreach (var tile in availableDestinations)
        {
            tile.MarkArrowPath(ArrowTranslator.ArrowDirection.None);
        }

        for (int i = 0; i < path.Count; i++)
        {
            var previousTile = i > 0 ? path[i - 1] : UnitReference.Tile;
            var futureTile = i < path.Count - 1 ? path[i + 1] : null;

            var arrow = tileGrid.ArrowTranslator.TranslateDirection(previousTile, path[i], futureTile);
            path[i].MarkArrowPath(arrow);
        }
    }

    private void ResetArrows()
    {
        path?.ForEach(t => t.MarkArrowPath(ArrowTranslator.ArrowDirection.None));
    }

    
}


