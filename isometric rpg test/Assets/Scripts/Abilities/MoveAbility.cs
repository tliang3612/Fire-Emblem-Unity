using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveAbility : Ability
{
    public OverlayTile Destination { get; set; }
    public List<OverlayTile> availableDestinations { get; set; }
    private List<OverlayTile> path;
        

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if(CanPerform(tileGrid) && availableDestinations.Contains(Destination))
        {
            var path = UnitReference.FindPath(Destination, tileGrid);
            UnitReference.Move(Destination, path);
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
            //Marks the reachable destinations
            availableDestinations.ForEach(t => t.MarkAsReachable());

        }
    }

    public override void OnUnitClicked(Unit unit, TileGrid tileGrid)
    {
        if (tileGrid.GetCurrentPlayerUnits().Contains(unit))
        {
            tileGrid.GridState = new TileGridStateAbilitySelected(tileGrid, unit, unit.GetComponents<Ability>().ToList());
        }
    }

    public override void OnTileClicked(OverlayTile tile, TileGrid tileGrid)
    {
        if (availableDestinations.Contains(tile) && UnitReference.IsTileMovableTo(tile))
        {
            Destination = tile;
            StartCoroutine(HumanExecute(tileGrid));
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
        else
        {
            ResetArrows();
        }
    }

    public override void OnTileDeselected(OverlayTile tile, TileGrid tileGrid)
    {
        if (CanPerform(tileGrid) && availableDestinations.Contains(tile))
        {
            availableDestinations.ForEach(t => t.MarkAsReachable());
        }
        else
        {
            ResetArrows();
        }
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        availableDestinations = UnitReference.GetAvailableDestinations(tileGrid);
    }

    public override void CleanUp(TileGrid tileGrid)
    {
        availableDestinations.ForEach(t => t.UnMark());
        ResetArrows();
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return UnitReference.ActionPoints > 0 && UnitReference.GetAvailableDestinations(tileGrid).Count > 1;
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


